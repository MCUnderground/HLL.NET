using HLL.NET.Models;

namespace HLL.NET.Examples
{
    public static class AccuracyTester
    {
        public static void RunAllTests()
        {
            Console.WriteLine("Running HLL.NET Accuracy Tests...\n");

            int[] cardinalities = { 100, 1_000, 10_000, 100_000 };
            int[] precisions = { 10, 12, 14 };
            int trials = 50;

            foreach (var precision in precisions)
            {
                Console.WriteLine($"Precision: {precision}");
                foreach (var count in cardinalities)
                {
                    var result = RunAccuracyTrial(count, precision, trials);
                    Console.WriteLine($"  Cardinality: {count,8:N0} | Avg Error: {result.AverageError:P3} | Min: {result.MinError:P3}, Max: {result.MaxError:P3}");
                }
                Console.WriteLine();
            }

            RunDuplicateTest();
        }

        private static (double AverageError, double MinError, double MaxError) RunAccuracyTrial(int cardinality, int precision, int trials)
        {
            var errors = new List<double>();

            for (int t = 0; t < trials; t++)
            {
                var hll = new HyperLogLog<string>(new HllPrecision(precision));
                for (int i = 0; i < cardinality; i++)
                    hll.Add($"user_{t}_{i}");

                double estimate = hll.Estimate();
                double error = Math.Abs(estimate - cardinality) / cardinality;
                errors.Add(error);
            }

            return (
                AverageError: errors.Average(),
                MinError: errors.Min(),
                MaxError: errors.Max()
            );
        }

        private static void RunDuplicateTest()
        {
            Console.WriteLine("Duplicate Handling Test:");

            int uniqueCount = 10_000;
            int duplicateCount = 2_000;

            var hll = new HyperLogLog<string>(new HllPrecision(14));
            for (int i = 0; i < uniqueCount; i++)
                hll.Add($"dup_{i}");
            for (int i = 0; i < duplicateCount; i++)
                hll.Add($"dup_{i % 500}"); // Only 500 unique duplicates

            double estimate = hll.Estimate();
            double error = Math.Abs(estimate - uniqueCount) / uniqueCount;

            Console.WriteLine($"  Unique + Duplicates: Estimated = {estimate:N0}, Actual = {uniqueCount}, Error = {error:P3}\n");
        }
    }
}
