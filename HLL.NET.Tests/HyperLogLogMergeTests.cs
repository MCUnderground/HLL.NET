using HLL.NET.Hashing;
using HLL.NET.Models;

namespace HLL.NET.Tests
{
    public class HyperLogLogMergeTests
    {
        private readonly IHasher<string> _hasher = new StringHasher();

        [Fact]
        public void Merge_WithDifferentPrecisions_ShouldThrow()
        {
            var hll1 = new HyperLogLog<string>(12, _hasher);
            var hll2 = new HyperLogLog<string>(14, _hasher);

            Assert.Throws<InvalidOperationException>(() => hll1.Merge(hll2));
        }

        [Fact]
        public void Merge_WithSamePrecision_ShouldCombineRegisters()
        {
            var hll1 = new HyperLogLog<string>(12, _hasher);
            var hll2 = new HyperLogLog<string>(12, _hasher);

            for (int i = 0; i < 500; i++)
                hll1.Add($"userA_{i}");

            for (int i = 0; i < 700; i++)
                hll2.Add($"userB_{i}");

            double estimate1 = hll1.Estimate();
            double estimate2 = hll2.Estimate();

            hll1.Merge(hll2);

            double mergedEstimate = hll1.Estimate();

            // Merged estimate should be at least as large as either individual estimate
            Assert.True(mergedEstimate >= estimate1, $"Merged estimate {mergedEstimate} is less than estimate1 {estimate1}");
            Assert.True(mergedEstimate >= estimate2, $"Merged estimate {mergedEstimate} is less than estimate2 {estimate2}");
        }
    }
}
