using UnityEngine;
using VTS_HDOutput.Core;

namespace VTS_HDOutput.Runtime
{
    internal sealed class HdCameraRenderer
    {
        public void Render(Camera camera, RenderTexture target, CompositionPlan plan)
        {
            if (camera == null || target == null) return;

            var previousTarget = camera.targetTexture;
            var previousClearFlags = camera.clearFlags;
            var previousBackground = camera.backgroundColor;

            try
            {
                camera.targetTexture = target;
                camera.ResetAspect();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0f, 0f, 0f, 0f);

                camera.Render();
            }
            finally
            {
                camera.targetTexture = previousTarget;
                camera.ResetAspect();
                camera.clearFlags = previousClearFlags;
                camera.backgroundColor = previousBackground;
            }
        }
    }
}
