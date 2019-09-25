namespace ByteStrings
{
    public static class Utils
    {
        public static int Align4(int count)
        {
            var remainder = count % 4;
            return count + remainder;
        }
        
        public static int Align16(int count)
        {
            var remainder = count % 16;
            return count + remainder;
        }
    }
}