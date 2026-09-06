using System;
using VTS_HDOutput.Core;
using Xunit;

public sealed class OutputTransportTests
{
    [Fact]
    public void Transport_options_only_include_spout()
    {
        Assert.Equal(new[] { OutputTransport.Spout }, Enum.GetValues<OutputTransport>());
    }

    [Fact]
    public void Spout_transport_is_enabled()
    {
        Assert.True(OutputTransportInfo.UsesSpout(OutputTransport.Spout));
    }
}
