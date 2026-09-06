using System;
using UnityEngine;
using VTS_HDOutput.Interop;

namespace VTS_HDOutput.Runtime
{
    internal sealed class SpoutOutput
    {
        private KlakSpoutSender sender;
        private string senderName;
        private int width;
        private int height;
        private IntPtr texturePointer;

        public string Status { get; private set; } = "Stopped";
        public Exception LastError { get; private set; }

        public void Ensure(string name, RenderTexture texture)
        {
            if (texture == null) return;

            var safeName = string.IsNullOrWhiteSpace(name) ? "VTS_HDOutput" : name;
            var pointer = texture.GetNativeTexturePtr();
            if (sender != null && senderName == safeName && width == texture.width && height == texture.height && texturePointer == pointer)
                return;

            Release();

            sender = new KlakSpoutSender(safeName, texture);
            senderName = safeName;
            width = texture.width;
            height = texture.height;
            texturePointer = pointer;
            LastError = null;
            Status = $"Sending {senderName} {width}x{height}";
        }

        public void Update()
        {
            sender?.Update();
        }

        public void Release()
        {
            sender?.Dispose();
            sender = null;
            senderName = null;
            width = 0;
            height = 0;
            texturePointer = IntPtr.Zero;
            Status = "Stopped";
        }

        public void Fail(Exception ex)
        {
            var message = ex.GetType().Name + ": " + ex.Message;
            LastError = ex;
            Release();
            LastError = ex;
            Status = message;
        }
    }
}
