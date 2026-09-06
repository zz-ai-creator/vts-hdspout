using VTS_HDOutput.Core;
using Xunit;

public sealed class OutputFrameRateTests
{
    [Theory]
    [InlineData(OutputFrameRate.Fps30, 30)]
    [InlineData(OutputFrameRate.Fps60, 60)]
    public void ToInt_maps_supported_frame_rates(OutputFrameRate frameRate, int expected)
    {
        Assert.Equal(expected, OutputFrameRateInfo.ToInt(frameRate));
    }
}
