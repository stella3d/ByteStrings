namespace ByteStrings
{
    public static class Utils
    {
        public static int Align4(int count)
        {
            var remainder = count % 4;
            return count + remainder;
        }
    }
}