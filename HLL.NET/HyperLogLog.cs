using HLL.NET.Hashing;
using HLL.NET.Models;
using System.Linq;
using System;
using HLL.NET.Maths;
using System.Text;
using HLL.NET.Serialization;

namespace HLL.NET
{
    public class HyperLogLog<T> 
    {
        internal HllRegister[] Registers => _registers;
        internal HllPrecision Precision => _precision;

        private readonly HllPrecision _precision;
        private readonly int _numRegisters;
        private readonly HllRegister[] _registers;

        private readonly IHasher<T> _hasher;

        public HyperLogLog(IHasher<T> hasher = null) : this(new HllPrecision(14), hasher) { }
        public HyperLogLog(HllPrecision precision, IHasher<T> hasher = null)
        {
            _hasher = hasher == null ? HasherFactory.GetDefault<T>() : hasher;

            if (precision < 4 || precision > 16)
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be between 4 and 16");

            _precision = precision;
            _numRegisters = 1 << _precision;
            _registers = new HllRegister[_numRegisters];

            for (int i = 0; i < _numRegisters; i++)
                _registers[i] = new HllRegister();
        }

        public void Add(T item)
        {
            var hash = _hasher.Hash(item);
            var index = (int)(hash >> (64 - _precision));
            var w = hash << _precision;
            var leadingZeros = BitUtils.CountLeadingZeros(w) + 1;

            _registers[index].Update((byte)leadingZeros);
        }

        public void Merge(HyperLogLog<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Precision != other.Precision)
                throw new InvalidOperationException("Cannot merge HyperLogLogs with different precisions.");

            for (int i = 0; i < _numRegisters; i++)
            {
                _registers[i].Update(other._registers[i].Value);
            }

        }

        public ulong Count => (ulong)Math.Round(Estimate());

        public double Estimate()
        {
            double alphaMM = GetAlphaMM();
            double sum = 0.0;

            foreach (var reg in _registers)
                sum += 1.0 / Math.Pow(2.0, reg.Value);

            double estimate = alphaMM / sum;
            if (double.IsNaN(estimate) || double.IsInfinity(estimate))
                return 0;


            // Bias correction
            if (estimate <= (5.0 / 2.0) * _numRegisters)
            {
                int zeroRegisters = _registers.Count(r => r.Value == 0);
                if (zeroRegisters != 0)
                    estimate = _numRegisters * Math.Log((double)_numRegisters / zeroRegisters);
            }

            return estimate;
        }

        private double GetAlphaMM()
        {
            switch (_numRegisters)
            {
                case 16: return 0.673 * _numRegisters * _numRegisters;
                case 32: return 0.697 * _numRegisters * _numRegisters;
                case 64: return 0.709 * _numRegisters * _numRegisters;
                default: return (0.7213 / (1 + 1.079 / _numRegisters)) * _numRegisters * _numRegisters;
            }
        }
        public byte[] Serialize() => HllSerializer.Serialize(this);

        public static HyperLogLog<T> Deserialize(byte[] data, IHasher<T> hasher) =>
            HllSerializer.Deserialize<T>(data, hasher);
    }
}
