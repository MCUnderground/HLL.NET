namespace HLL.NET.Maths
{
    internal static class BitUtils
    {
        public static int CountLeadingZeros(ulong value)
        {
            if (value == 0) return 64;
            int count = 0;
            while ((value & (1UL << 63)) == 0)
            {
                value <<= 1;
                count++;
            }
            return count;
        }
    }
}
