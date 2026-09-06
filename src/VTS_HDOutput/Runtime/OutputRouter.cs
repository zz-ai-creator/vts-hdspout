using System;
using UnityEngine;
using VTS_HDOutput.Core;

namespace VTS_HDOutput.Runtime
{
    internal sealed class OutputRouter
    {
        private readonly SpoutOutput spoutOutput = new SpoutOutput();

        public string Status { get; private set; } = "Stopped";

        public void Ensure(OutputTransport transport, string senderName, RenderTexture texture)
        {
            var spoutStatus = string.Empty;

            if (OutputTransportInfo.UsesSpout(transport))
            {
                try
                {
                    spoutOutput.Ensure(senderName, texture);
                    spoutStatus = spoutOutput.Status;
                }
                catch (Exception ex)
                {
                    spoutOutput.Fail(ex);
                    spoutStatus = spoutOutput.Status;
                }
            }
            else
            {
                spoutOutput.Release();
            }

            Status = CombineStatus(spoutStatus);
        }

        public void Update()
        {
            spoutOutput.Update();
        }

        public void Release()
        {
            spoutOutput.Release();
            Status = "Stopped";
        }

        private static string CombineStatus(string spoutStatus)
        {
            if (!string.IsNullOrEmpty(spoutStatus)) return spoutStatus;
            return "Stopped";
        }
    }
}
