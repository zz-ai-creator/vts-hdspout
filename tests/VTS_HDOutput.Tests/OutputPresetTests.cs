using VTS_HDOutput.Core;
using Xunit;

public sealed class OutputPresetTests
{
    [Theory]
    [InlineData(HdOutputPreset.Landscape1080p, 1920, 1080, false)]
    [InlineData(HdOutputPreset.Landscape2K, 2560, 1440, false)]
    [InlineData(HdOutputPreset.Landscape4K, 3840, 2160, false)]
    [InlineData(HdOutputPreset.Portrait1080p, 1080, 1920, true)]
    [InlineData(HdOutputPreset.Portrait2K, 1440, 2560, true)]
    [InlineData(HdOutputPreset.Portrait4K, 2160, 3840, true)]
    public void Describe_returns_expected_dimensions(HdOutputPreset preset, int width, int height, bool portrait)
    {
        var desc = OutputPresetInfo.Describe(preset);

        Assert.Equal(width, desc.Width);
        Assert.Equal(height, desc.Height);
        Assert.Equal(portrait, desc.IsPortrait);
        Assert.Equal((float)width / height, desc.Aspect, 4);
    }

    [Theory]
    [InlineData(OutputAspectRatio.Ratio9x16, OutputResolution.P1080, 1080, 1920, true)]
    [InlineData(OutputAspectRatio.Ratio9x16, OutputResolution.K2, 1440, 2560, true)]
    [InlineData(OutputAspectRatio.Ratio9x16, OutputResolution.K4, 2160, 3840, true)]
    [InlineData(OutputAspectRatio.Ratio16x9, OutputResolution.P1080, 1920, 1080, false)]
    [InlineData(OutputAspectRatio.Ratio16x9, OutputResolution.K2, 2560, 1440, false)]
    [InlineData(OutputAspectRatio.Ratio16x9, OutputResolution.K4, 3840, 2160, false)]
    [InlineData(OutputAspectRatio.Ratio3x4, OutputResolution.P1080, 1080, 1440, true)]
    [InlineData(OutputAspectRatio.Ratio3x4, OutputResolution.K2, 1440, 1920, true)]
    [InlineData(OutputAspectRatio.Ratio3x4, OutputResolution.K4, 2160, 2880, true)]
    [InlineData(OutputAspectRatio.Ratio4x3, OutputResolution.P1080, 1440, 1080, false)]
    [InlineData(OutputAspectRatio.Ratio4x3, OutputResolution.K2, 1920, 1440, false)]
    [InlineData(OutputAspectRatio.Ratio4x3, OutputResolution.K4, 2880, 2160, false)]
    public void Describe_returns_expected_dimensions_for_aspect_ratio_and_resolution(
        OutputAspectRatio aspectRatio,
        OutputResolution resolution,
        int width,
        int height,
        bool portrait)
    {
        var desc = OutputPresetInfo.Describe(aspectRatio, resolution);

        Assert.Equal(aspectRatio, desc.AspectRatio);
        Assert.Equal(resolution, desc.Resolution);
        Assert.Equal(width, desc.Width);
        Assert.Equal(height, desc.Height);
        Assert.Equal(portrait, desc.IsPortrait);
        Assert.Equal((float)width / height, desc.Aspect, 4);
    }
}
