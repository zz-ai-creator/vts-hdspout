using VTS_HDOutput.Core;
using Xunit;

public sealed class RenderCadenceTests
{
    [Fact]
    public void ShouldRender_returns_true_when_target_fps_is_unlimited()
    {
        Assert.True(RenderCadence.ShouldRender(12.5, 99, 0));
        Assert.True(RenderCadence.ShouldRender(12.5, 99, -1));
    }

    [Fact]
    public void ShouldRender_skips_until_next_scheduled_time()
    {
        Assert.False(RenderCadence.ShouldRender(1.20, 1.25, 30));
        Assert.True(RenderCadence.ShouldRender(1.25, 1.25, 30));
        Assert.True(RenderCadence.ShouldRender(1.30, 1.25, 30));
    }

    [Theory]
    [InlineData(30, 1.0 / 30.0)]
    [InlineData(60, 1.0 / 60.0)]
    [InlineData(240, 1.0 / 240.0)]
    public void NextRenderAt_adds_the_target_frame_interval(int targetFps, double interval)
    {
        Assert.Equal(10.0 + interval, RenderCadence.NextRenderAt(10.0, targetFps), 6);
    }

    [Theory]
    [InlineData(OutputFrameRate.Fps30, 1.0 / 30.0)]
    [InlineData(OutputFrameRate.Fps60, 1.0 / 60.0)]
    public void NextRenderAt_accepts_output_frame_rate(OutputFrameRate frameRate, double interval)
    {
        Assert.Equal(20.0 + interval, RenderCadence.NextRenderAt(20.0, frameRate), 6);
    }
}
