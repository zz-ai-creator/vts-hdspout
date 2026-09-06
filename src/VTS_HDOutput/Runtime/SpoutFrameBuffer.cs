using UnityEngine;

namespace VTS_HDOutput.Runtime
{
    internal sealed class SpoutFrameBuffer
    {
        public RenderTexture Texture { get; private set; }

        public RenderTexture Prepare(RenderTexture source, bool flipVertical)
        {
            if (source == null) return null;

            if (!flipVertical)
            {
                Release();
                return source;
            }

            Ensure(source.width, source.height);

            var previousActive = RenderTexture.active;
            Graphics.Blit(source, Texture, new Vector2(1f, -1f), new Vector2(0f, 1f));
            RenderTexture.active = previousActive;

            return Texture;
        }

        public void Release()
        {
            if (Texture == null) return;

            Texture.Release();
            Object.Destroy(Texture);
            Texture = null;
        }

        private void Ensure(int width, int height)
        {
            if (Texture != null && Texture.width == width && Texture.height == height) return;

            Release();

            Texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
            {
                name = $"VTS_HDOutput_Send_{width}x{height}",
                hideFlags = HideFlags.DontSave,
                antiAliasing = 1,
                useMipMap = false,
                autoGenerateMips = false
            };
            Texture.Create();
        }
    }
}
