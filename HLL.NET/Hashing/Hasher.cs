using System;
using System.Security.Cryptography;
using System.Text;

namespace HLL.NET.Hashing
{
    internal static class Hasher
    {
        public static ulong Hash(string value)
        {
            using (var hasher = SHA256.Create())
            {
                byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(value));
                ulong result = 0;
                for (int i = 0; i < 4; i++)
                    result ^= BitConverter.ToUInt64(hashBytes, i * 8);
                return result;
            }
        }
    }
}
