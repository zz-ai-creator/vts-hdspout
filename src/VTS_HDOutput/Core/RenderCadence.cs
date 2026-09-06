namespace VTS_HDOutput.Core
{
    public static class RenderCadence
    {
        private const int MaxTargetFps = 240;

        public static bool ShouldRender(double now, double nextRenderAt, int targetFps)
        {
            if (targetFps <= 0) return true;

            return now >= nextRenderAt;
        }

        public static double NextRenderAt(double now, int targetFps)
        {
            if (targetFps <= 0) return now;

            var safeTargetFps = targetFps > MaxTargetFps ? MaxTargetFps : targetFps;
            return now + 1d / safeTargetFps;
        }

        public static double NextRenderAt(double now, OutputFrameRate frameRate)
        {
            return NextRenderAt(now, OutputFrameRateInfo.ToInt(frameRate));
        }
    }
}
