using VTS_HDOutput.Core;
using Xunit;

public sealed class ToolbarLayoutTests
{
    [Fact]
    public void Centered_places_toolbar_at_top_center()
    {
        var rect = ToolbarLayout.Centered(1920, 760, 10);

        Assert.Equal(580, rect.X);
        Assert.Equal(10, rect.Y);
        Assert.Equal(760, rect.Width);
    }

    [Fact]
    public void Centered_clamps_to_left_edge_when_screen_is_narrow()
    {
        var rect = ToolbarLayout.Centered(500, 760, 12);

        Assert.Equal(0, rect.X);
        Assert.Equal(12, rect.Y);
        Assert.Equal(760, rect.Width);
    }

    [Fact]
    public void Centered_includes_grouped_toolbar_height()
    {
        var rect = ToolbarLayout.Centered(1920, 980, 92, 10);

        Assert.Equal(470, rect.X);
        Assert.Equal(10, rect.Y);
        Assert.Equal(980, rect.Width);
        Assert.Equal(92, rect.Height);
    }

    [Fact]
    public void TopLeft_places_toolbar_at_fixed_margin()
    {
        var rect = ToolbarLayout.TopLeft(480, 164, 10, 10);

        Assert.Equal(10, rect.X);
        Assert.Equal(10, rect.Y);
        Assert.Equal(480, rect.Width);
        Assert.Equal(164, rect.Height);
    }
}
