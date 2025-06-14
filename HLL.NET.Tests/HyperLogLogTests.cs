namespace HLL.NET.Tests
{
    public class HyperLogLogTests
    {
        [Fact]
        public void Estimate_UniqueValues_ShouldBeCloseToActual()
        {
            var items = new List<string>();
            for (int i = 0; i < 10_000; i++)
                items.Add($"user_{i}");

            double estimated = HyperLogLogEstimator.EstimateWithMultipleRuns(items, runs: 5, precision: 14);

            int actual = items.Distinct().Count();
            double error = System.Math.Abs(actual - estimated) / actual;

            Assert.InRange(error, 0, 0.01); // Accept up to 1% error
        }

        [Fact]
        public void Estimate_WithDuplicates_ShouldStillBeAccurate()
        {
            var items = new List<string>();
            for (int i = 0; i < 10_000; i++)
                items.Add($"user_{i}");
            for (int i = 0; i < 1_000; i++)
                items.Add($"user_{i % 500}"); // Duplicates

            double estimated = HyperLogLogEstimator.EstimateWithMultipleRuns(items, runs: 3, precision: 14);
            int actual = items.Distinct().Count();
            double error = System.Math.Abs(actual - estimated) / actual;

            Assert.InRange(error, 0, 0.01);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(10)]
        [InlineData(14)]
        [InlineData(16)]
        public void Constructor_AllowsValidPrecisions(int precision)
        {
            var hll = new HyperLogLog<int>(precision);
            Assert.NotNull(hll);
        }

        [Fact]
        public void Constructor_ThrowsForInvalidPrecision()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HyperLogLog<string>(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new HyperLogLog<string>(17));
        }

        [Fact]
        public void HandlesDuplicates()
        {
            var hll = new HyperLogLog<string>();
            for (int i = 0; i < 10000; i++)
                hll.Add("same_value");

            var estimate = hll.Estimate();

            // Use Assert.True for float tolerance in xUnit
            Assert.True(Math.Abs(estimate - 1.0) < 0.1, $"Estimate was {estimate}, expected ~1.0");
        }

    }
}
