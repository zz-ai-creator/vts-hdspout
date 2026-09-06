using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;

namespace VTS_HDOutput.Interop
{
    internal enum KlakSpoutEventId
    {
        UpdateSender,
        UpdateReceiver,
        CloseSender,
        CloseReceiver
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KlakSpoutEventData
    {
        public IntPtr InstancePointer;
        public IntPtr TexturePointer;

        public KlakSpoutEventData(IntPtr instancePointer, IntPtr texturePointer)
        {
            InstancePointer = instancePointer;
            TexturePointer = texturePointer;
        }
    }

    internal sealed class KlakSpoutEventKicker : IDisposable
    {
        private static CommandBuffer commandBuffer;
        private GCHandle dataHandle;

        public KlakSpoutEventKicker(KlakSpoutEventData data)
        {
            dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        }

        public void Issue(KlakSpoutEventId eventId)
        {
            if (commandBuffer == null)
                commandBuffer = new CommandBuffer { name = "VTS_HDOutput.KlakSpoutEvent" };
            else
                commandBuffer.Clear();

            commandBuffer.IssuePluginEventAndData(
                KlakSpoutNative.GetRenderEventCallback(),
                (int)eventId,
                dataHandle.AddrOfPinnedObject());

            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        public void Dispose()
        {
            KlakSpoutMemoryPool.FreeOnEndOfFrame(dataHandle);
        }
    }

    internal static class KlakSpoutMemoryPool
    {
        private static readonly Stack<GCHandle> Handles = new Stack<GCHandle>();
        private static bool installed;

        public static void EnsureInstalled()
        {
            if (installed) return;
            installed = true;

            var customSystem = new PlayerLoopSystem
            {
                type = typeof(KlakSpoutMemoryPool),
                updateDelegate = ReleaseHandles
            };

            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            for (var i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.PostLateUpdate)) continue;

                var current = playerLoop.subSystemList[i].subSystemList ?? new PlayerLoopSystem[0];
                playerLoop.subSystemList[i].subSystemList = current.Concat(new[] { customSystem }).ToArray();
                PlayerLoop.SetPlayerLoop(playerLoop);
                return;
            }
        }

        public static void FreeOnEndOfFrame(GCHandle handle)
        {
            EnsureInstalled();
            Handles.Push(handle);
        }

        private static void ReleaseHandles()
        {
            while (Handles.Count > 0)
                Handles.Pop().Free();
        }
    }

    internal sealed class KlakSpoutSender : IDisposable
    {
        private IntPtr pluginPointer;
        private KlakSpoutEventKicker eventKicker;
        private readonly string senderName;
        private readonly int width;
        private readonly int height;

        public KlakSpoutSender(string senderName, Texture texture)
        {
            this.senderName = string.IsNullOrWhiteSpace(senderName) ? "VTS_HDOutput" : senderName;
            width = texture.width;
            height = texture.height;

            KlakSpoutMemoryPool.EnsureInstalled();

            pluginPointer = KlakSpoutNative.CreateSender(this.senderName, width, height);
            if (pluginPointer == IntPtr.Zero)
                throw new InvalidOperationException("KlakSpout CreateSender returned IntPtr.Zero.");

            eventKicker = new KlakSpoutEventKicker(new KlakSpoutEventData(pluginPointer, texture.GetNativeTexturePtr()));
            eventKicker.Issue(KlakSpoutEventId.UpdateSender);
        }

        public string SenderName => senderName;
        public int Width => width;
        public int Height => height;

        public void Update()
        {
            eventKicker?.Issue(KlakSpoutEventId.UpdateSender);
        }

        public void Dispose()
        {
            if (pluginPointer == IntPtr.Zero) return;

            eventKicker.Issue(KlakSpoutEventId.CloseSender);
            eventKicker.Dispose();
            eventKicker = null;
            pluginPointer = IntPtr.Zero;
        }
    }
}
