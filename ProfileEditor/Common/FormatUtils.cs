namespace ProfileEditor.Common
{
    public static class FormatUtils
    {
        public static int ToVatsimFrequencyFormat(this decimal value)
        {
            return (int)((value - 100m) * 1000m);
        }
    }
}
