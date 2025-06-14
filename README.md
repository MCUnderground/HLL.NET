# HLL.NET – HyperLogLog for .NET

[![NuGet](https://img.shields.io/nuget/v/HLL.NET.svg?label=NuGet&color=blue)](https://www.nuget.org/packages/HLL.NET)
[![Tests](https://github.com/MCUnderground/HLL.NET/actions/workflows/tests.yml/badge.svg)](https://github.com/MCUnderground/HLL.NET/actions)

A fast and memory-efficient implementation of the [HyperLogLog](https://en.wikipedia.org/wiki/HyperLogLog) algorithm in .NET for approximate cardinality estimation of large data sets.

---

## 🚀 Features

- ⚡ Efficient cardinality estimation with HyperLogLog algorithm
- 🧠 Built-in support for multiple types: string, int, Guid, and more via custom hashers
- 🔧 Easy to extend with your own IHasher<T> implementations for any data type
- 🧪 Optional multiple-run estimation for improved accuracy and reduced variance
- 🧱 Configurable precision (4–16) with built-in validation
- 💼 Fully compatible with .NET Standard for broad platform support

---

## 📦 Installation

```bash
dotnet add package HLL.NET
```

---

## 🧑‍💻 Usage

```csharp
using HLL.NET;

var items = new List<string>();
for (int i = 0; i < 10_000; i++)
    items.Add($"user_{i}");

double estimate = HyperLogLog.EstimateWithMultipleRuns(items, runs: 5, precision: 14);

Console.WriteLine($"Estimated unique count: {estimate:F2}");
```

For more fine-grained control:

```csharp
var hll = new HyperLogLog<string>(precision: 14);

hll.Add("apple");
hll.Add("banana");
hll.Add("apple");

double estimate = hll.Estimate();
Console.WriteLine($"Estimated: {estimate:F2}");
```

---

## ✅ Running Tests

```bash
dotnet test
```

Tests are located in the `tests/HLL.NET.Tests/` project and cover various edge cases and expected behaviors.

---

## 🤝 Contributing

PRs and suggestions welcome! Please:

1. Fork the repo
2. Create a feature branch
3. Add tests if needed
4. Submit a PR 🚀

---

## 📄 License

MIT © 2025 — [MCUnderground](https://github.com/MCUnderground)
