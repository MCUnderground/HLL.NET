using System;

namespace HLL.NET.Hashing
{
    public class GuidHasher : IHasher<Guid>
    {
        public ulong Hash(Guid value)
        {
            byte[] bytes = value.ToByteArray();
            ulong result = 0;
            for (int i = 0; i < 2; i++)
                result ^= BitConverter.ToUInt64(bytes, i * 8);
            return result;
        }
    }

}
