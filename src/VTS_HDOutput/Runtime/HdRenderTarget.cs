using UnityEngine;

namespace VTS_HDOutput.Runtime
{
    internal sealed class HdRenderTarget
    {
        public RenderTexture Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Ensure(int width, int height)
        {
            if (Texture != null && Width == width && Height == height) return false;

            Release();

            Width = width;
            Height = height;
            Texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                name = $"VTS_HDOutput_{width}x{height}",
                hideFlags = HideFlags.DontSave,
                antiAliasing = 1,
                useMipMap = false,
                autoGenerateMips = false
            };
            Texture.Create();
            return true;
        }

        public void Release()
        {
            if (Texture == null) return;

            Texture.Release();
            Object.Destroy(Texture);
            Texture = null;
            Width = 0;
            Height = 0;
        }
    }
}
