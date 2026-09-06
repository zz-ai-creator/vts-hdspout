namespace VTS_HDOutput.Core
{
    public enum HdOutputPreset
    {
        Landscape1080p,
        Landscape2K,
        Landscape4K,
        Portrait1080p,
        Portrait2K,
        Portrait4K
    }

    public enum OutputAspectRatio
    {
        Ratio9x16,
        Ratio16x9,
        Ratio3x4,
        Ratio4x3
    }

    public enum OutputResolution
    {
        P1080,
        K2,
        K4
    }

    public readonly struct OutputPresetDescriptor
    {
        public OutputPresetDescriptor(HdOutputPreset preset, int width, int height)
            : this(preset, AspectRatioForPreset(preset), ResolutionForPreset(preset), width, height)
        {
        }

        public OutputPresetDescriptor(OutputAspectRatio aspectRatio, OutputResolution resolution, int width, int height)
            : this(HdOutputPreset.Landscape1080p, aspectRatio, resolution, width, height)
        {
        }

        private OutputPresetDescriptor(
            HdOutputPreset preset,
            OutputAspectRatio aspectRatio,
            OutputResolution resolution,
            int width,
            int height)
        {
            Preset = preset;
            AspectRatio = aspectRatio;
            Resolution = resolution;
            Width = width;
            Height = height;
        }

        public HdOutputPreset Preset { get; }
        public OutputAspectRatio AspectRatio { get; }
        public OutputResolution Resolution { get; }
        public int Width { get; }
        public int Height { get; }
        public bool IsPortrait => Height > Width;
        public float Aspect => (float)Width / Height;

        private static OutputAspectRatio AspectRatioForPreset(HdOutputPreset preset)
        {
            switch (preset)
            {
                case HdOutputPreset.Portrait1080p:
                case HdOutputPreset.Portrait2K:
                case HdOutputPreset.Portrait4K:
                    return OutputAspectRatio.Ratio9x16;
                case HdOutputPreset.Landscape1080p:
                case HdOutputPreset.Landscape2K:
                case HdOutputPreset.Landscape4K:
                default:
                    return OutputAspectRatio.Ratio16x9;
            }
        }

        private static OutputResolution ResolutionForPreset(HdOutputPreset preset)
        {
            switch (preset)
            {
                case HdOutputPreset.Landscape4K:
                case HdOutputPreset.Portrait4K:
                    return OutputResolution.K4;
                case HdOutputPreset.Landscape2K:
                case HdOutputPreset.Portrait2K:
                    return OutputResolution.K2;
                case HdOutputPreset.Landscape1080p:
                case HdOutputPreset.Portrait1080p:
                default:
                    return OutputResolution.P1080;
            }
        }
    }

    public static class OutputPresetInfo
    {
        public static OutputPresetDescriptor Describe(HdOutputPreset preset)
        {
            switch (preset)
            {
                case HdOutputPreset.Landscape1080p:
                    return new OutputPresetDescriptor(preset, 1920, 1080);
                case HdOutputPreset.Landscape2K:
                    return new OutputPresetDescriptor(preset, 2560, 1440);
                case HdOutputPreset.Landscape4K:
                    return new OutputPresetDescriptor(preset, 3840, 2160);
                case HdOutputPreset.Portrait1080p:
                    return new OutputPresetDescriptor(preset, 1080, 1920);
                case HdOutputPreset.Portrait2K:
                    return new OutputPresetDescriptor(preset, 1440, 2560);
                case HdOutputPreset.Portrait4K:
                    return new OutputPresetDescriptor(preset, 2160, 3840);
                default:
                    return new OutputPresetDescriptor(HdOutputPreset.Landscape1080p, 1920, 1080);
            }
        }

        public static OutputPresetDescriptor Describe(OutputAspectRatio aspectRatio, OutputResolution resolution)
        {
            var shortEdge = ShortEdgeForResolution(resolution);

            switch (aspectRatio)
            {
                case OutputAspectRatio.Ratio9x16:
                    return new OutputPresetDescriptor(aspectRatio, resolution, shortEdge, shortEdge * 16 / 9);
                case OutputAspectRatio.Ratio16x9:
                    return new OutputPresetDescriptor(aspectRatio, resolution, shortEdge * 16 / 9, shortEdge);
                case OutputAspectRatio.Ratio3x4:
                    return new OutputPresetDescriptor(aspectRatio, resolution, shortEdge, shortEdge * 4 / 3);
                case OutputAspectRatio.Ratio4x3:
                    return new OutputPresetDescriptor(aspectRatio, resolution, shortEdge * 4 / 3, shortEdge);
                default:
                    return new OutputPresetDescriptor(OutputAspectRatio.Ratio16x9, resolution, shortEdge * 16 / 9, shortEdge);
            }
        }

        private static int ShortEdgeForResolution(OutputResolution resolution)
        {
            switch (resolution)
            {
                case OutputResolution.K4:
                    return 2160;
                case OutputResolution.K2:
                    return 1440;
                case OutputResolution.P1080:
                default:
                    return 1080;
            }
        }
    }
}
