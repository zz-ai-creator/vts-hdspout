namespace VTS_HDOutput.Core
{
    public enum FitMode
    {
        Fit,
        Fill
    }

    public readonly struct CompositionPlan
    {
        public CompositionPlan(float aspect, FitMode fitMode, float scale, float offsetX, float offsetY)
        {
            Aspect = aspect;
            FitMode = fitMode;
            Scale = scale;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public float Aspect { get; }
        public FitMode FitMode { get; }
        public float Scale { get; }
        public float OffsetX { get; }
        public float OffsetY { get; }
    }

    public static class CompositionMath
    {
        public static CompositionPlan BuildPlan(int width, int height, FitMode fitMode, float scale, float offsetX, float offsetY)
        {
            var safeWidth = width <= 0 ? 1920 : width;
            var safeHeight = height <= 0 ? 1080 : height;
            var safeScale = scale <= 0 ? 1f : scale;

            return new CompositionPlan((float)safeWidth / safeHeight, fitMode, safeScale, offsetX, offsetY);
        }
    }
}
