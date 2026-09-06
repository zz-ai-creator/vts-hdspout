namespace VTS_HDOutput.Core
{
    public enum OutputFrameRate
    {
        Fps30,
        Fps60
    }

    public static class OutputFrameRateInfo
    {
        public static int ToInt(OutputFrameRate frameRate)
        {
            return frameRate == OutputFrameRate.Fps60 ? 60 : 30;
        }
    }
}
