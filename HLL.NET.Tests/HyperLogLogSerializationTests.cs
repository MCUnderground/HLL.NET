using HLL.NET.Hashing;
using HLL.NET.Models;

namespace HLL.NET.Tests
{
    public class HyperLogLogSerializationTests
    {
        private readonly IHasher<string> _stringHasher = new StringHasher();

        [Fact]
        public void Serialize_ThenDeserialize_ShouldPreserveState()
        {
            var hll = new HyperLogLog<string>(new HllPrecision(14), _stringHasher);

            for (int i = 0; i < 1000; i++)
                hll.Add($"user_{i}");

            double estimateBefore = hll.Estimate();
            byte[] serialized = hll.Serialize();

            var deserialized = HyperLogLog<string>.Deserialize(serialized, _stringHasher);

            double estimateAfter = deserialized.Estimate();

            double diff = Math.Abs(estimateBefore - estimateAfter);
            Assert.InRange(diff, 0, 1.0);  // Allow minor floating point diff
        }

        [Fact]
        public void Deserialize_InvalidData_ShouldThrow()
        {
            byte[] invalidData = new byte[] { 0 }; // Too short

            Assert.Throws<ArgumentException>(() =>
                HyperLogLog<string>.Deserialize(invalidData, _stringHasher));
        }

        [Fact]
        public void Deserialize_UnsupportedVersion_ShouldThrow()
        {
            var hll = new HyperLogLog<string>(new HllPrecision(14), _stringHasher);
            var data = hll.Serialize();

            // Corrupt version byte
            data[0] = 99;

            Assert.Throws<NotSupportedException>(() =>
                HyperLogLog<string>.Deserialize(data, _stringHasher));
        }
    }
}

