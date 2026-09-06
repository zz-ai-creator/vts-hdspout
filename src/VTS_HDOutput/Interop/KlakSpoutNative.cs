using System;
using System.Runtime.InteropServices;

namespace VTS_HDOutput.Interop
{
    internal static class KlakSpoutNative
    {
        [DllImport("KlakSpout")]
        internal static extern IntPtr GetRenderEventCallback();

        [DllImport("KlakSpout")]
        internal static extern IntPtr CreateSender(string name, int width, int height);
    }
}
