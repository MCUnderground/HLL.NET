using System.Collections.Generic;
using System;
using HLL.NET.Hashing;

namespace HLL.NET
{
    public static class HyperLogLogEstimator
    {
        public static double EstimateWithMultipleRuns<T>(IEnumerable<T> items, int runs = 5, int precision = 14, IHasher<T> hasher = null)
        {
            if (runs <= 0) throw new ArgumentException("Runs must be > 0");

            double total = 0;
            for (int i = 0; i < runs; i++)
            {
                var hll = new HyperLogLog<T>(precision, hasher);
                foreach (var item in items)
                    hll.Add(item);
                total += hll.Estimate();
            }

            return total / runs;
        }
    }
}
