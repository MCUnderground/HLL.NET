using System;

namespace HLL.NET.Models
{
    public readonly struct HllPrecision
    {
        public int Value { get; }

        public HllPrecision(int value)
        {
            if (value < 4 || value > 16)
                throw new ArgumentOutOfRangeException(nameof(value), "Precision must be between 4 and 16.");

            Value = value;
        }

        public static implicit operator int(HllPrecision precision) => precision.Value;
        public static implicit operator HllPrecision(int value) => new HllPrecision(value);


        public override string ToString() => $"HllPrecision({Value})";
    }
}
