using VTS_HDOutput.Core;
using Xunit;

public sealed class CompositionMathTests
{
    [Theory]
    [InlineData(1920, 1080, FitMode.Fit)]
    [InlineData(1080, 1920, FitMode.Fit)]
    [InlineData(2160, 3840, FitMode.Fill)]
    public void BuildPlan_uses_output_aspect(int width, int height, FitMode fitMode)
    {
        var plan = CompositionMath.BuildPlan(width, height, fitMode, 1.25f, 0.1f, -0.2f);

        Assert.Equal((float)width / height, plan.Aspect, 4);
        Assert.Equal(1.25f, plan.Scale);
        Assert.Equal(0.1f, plan.OffsetX);
        Assert.Equal(-0.2f, plan.OffsetY);
        Assert.Equal(fitMode, plan.FitMode);
    }
}
