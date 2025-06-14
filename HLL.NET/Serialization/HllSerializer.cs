using HLL.NET.Hashing;
using HLL.NET.Models;
using System;
using System.Text;

namespace HLL.NET.Serialization
{

    //docs/serialization_format.md
    internal static class HllSerializer
    {
        private const byte FormatVersion = 2;

        public static byte[] Serialize<T>(HyperLogLog<T> hll)
        {
            var typeNameBytes = EncodeTypeName(typeof(T));
            byte typeNameLength = (byte)typeNameBytes.Length;

            var data = new byte[1 + 1 + typeNameLength + 1 + hll.Registers.Length];
            int index = 0;

            data[index++] = FormatVersion;
            data[index++] = typeNameLength;
            Array.Copy(typeNameBytes, 0, data, index, typeNameLength);
            index += typeNameLength;

            data[index++] = (byte)hll.Precision.Value;
            WriteRegisters(hll, data, ref index);

            return data;
        }

        public static HyperLogLog<T> Deserialize<T>(byte[] data, IHasher<T> hasher)
        {
            if (data == null || data.Length < 4)
                throw new ArgumentException("Invalid serialized data");

            int index = 0;

            ValidateVersion(data[index++]);

            var typeName = ReadTypeName(data, ref index);
            EnsureCorrectType<T>(typeName);

            var precision = new HllPrecision(data[index++]);
            var hll = new HyperLogLog<T>(precision, hasher);

            ReadRegisters(hll, data, ref index);

            return hll;
        }


        private static byte[] EncodeTypeName(Type type) =>
            Encoding.UTF8.GetBytes(type.FullName);

        private static void WriteRegisters<T>(HyperLogLog<T> hll, byte[] data, ref int index)
        {
            foreach (var reg in hll.Registers)
                data[index++] = reg.Value;
        }

        private static void ReadRegisters<T>(HyperLogLog<T> hll, byte[] data, ref int index)
        {
            for (int i = 0; i < hll.Registers.Length; i++)
                hll.Registers[i] = new HllRegister(data[index++]);
        }

        private static void ValidateVersion(byte version)
        {
            if (version != FormatVersion)
                throw new NotSupportedException($"Unsupported HLL format version: {version}");
        }

        private static string ReadTypeName(byte[] data, ref int index)
        {
            var length = data[index++];
            var typeName = Encoding.UTF8.GetString(data, index, length);
            index += length;
            return typeName;
        }

        private static void EnsureCorrectType<T>(string serializedTypeName)
        {
            var expected = typeof(T).FullName;
            if (serializedTypeName != expected)
                throw new InvalidOperationException($"Type mismatch. Serialized for '{serializedTypeName}', but deserializing as '{expected}'.");
        }
    }
}
