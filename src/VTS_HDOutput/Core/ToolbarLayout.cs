namespace VTS_HDOutput.Core
{
    public readonly struct ToolbarRect
    {
        public ToolbarRect(int x, int y, int width)
            : this(x, y, width, 0)
        {
        }

        public ToolbarRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
    }

    public static class ToolbarLayout
    {
        public static ToolbarRect Centered(int screenWidth, int toolbarWidth, int topMargin)
        {
            return Centered(screenWidth, toolbarWidth, 0, topMargin);
        }

        public static ToolbarRect Centered(int screenWidth, int toolbarWidth, int toolbarHeight, int topMargin)
        {
            var x = (screenWidth - toolbarWidth) / 2;
            if (x < 0) x = 0;

            return new ToolbarRect(x, topMargin, toolbarWidth, toolbarHeight);
        }

        public static ToolbarRect TopLeft(int toolbarWidth, int toolbarHeight, int leftMargin, int topMargin)
        {
            return new ToolbarRect(leftMargin, topMargin, toolbarWidth, toolbarHeight);
        }
    }
}
