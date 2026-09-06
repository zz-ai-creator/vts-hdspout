using UnityEngine;

namespace VTS_HDOutput.Runtime
{
    internal sealed class CameraResolver
    {
        private Camera cachedCamera;

        public Camera Resolve()
        {
            if (cachedCamera != null) return cachedCamera;

            var byPath = GameObject.Find("Cameras/Live2D Camera");
            if (byPath != null)
                cachedCamera = byPath.GetComponent<Camera>();

            if (cachedCamera != null) return cachedCamera;

            var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var camera in cameras)
            {
                if (camera.name == "Live2D Camera" || camera.transform.name == "Live2D Camera")
                {
                    cachedCamera = camera;
                    return cachedCamera;
                }
            }

            return null;
        }

        public void Clear()
        {
            cachedCamera = null;
        }
    }
}
