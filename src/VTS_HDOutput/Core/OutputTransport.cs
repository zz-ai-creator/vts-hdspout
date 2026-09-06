namespace VTS_HDOutput.Core
{
    public enum OutputTransport
    {
        Spout
    }

    public static class OutputTransportInfo
    {
        public static bool UsesSpout(OutputTransport transport)
        {
            return transport == OutputTransport.Spout;
        }
    }
}
