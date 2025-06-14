namespace HLL.NET.Hashing
{
    public class IntHasher : IHasher<int>
    {
        public ulong Hash(int value)
        {
            ulong hash = (ulong)value;
            hash ^= (hash >> 33);
            hash *= 0xff51afd7ed558ccd;
            hash ^= (hash >> 33);
            hash *= 0xc4ceb9fe1a85ec53;
            hash ^= (hash >> 33);
            return hash;
        }
    }

}
