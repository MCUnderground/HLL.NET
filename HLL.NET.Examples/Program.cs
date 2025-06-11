using HLL.NET;

var items = new List<string>();

for (int i = 0; i < 10_000; i++)
{
    items.Add($"user_{i}");
}
for (int i = 0; i < 1_000; i++)
{
    items.Add($"user_{i % 1000}");
}

int trueUniqueCount = items.Distinct().Count();
double estimatedCount = HyperLogLogEstimator.EstimateWithMultipleRuns(items, runs: 5, precision: 14);

Console.WriteLine($"True unique count:      {trueUniqueCount}");
Console.WriteLine($"Estimated unique count: {estimatedCount:F2}");
Console.WriteLine($"Error:                  {Math.Abs(trueUniqueCount - estimatedCount) / trueUniqueCount:P2}");
