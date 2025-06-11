using HLL.NET.Hashing;
using HLL.NET.Models;
using System.Linq;
using System;
using HLL.NET.Maths;

namespace HLL.NET
{
    public class HyperLogLog
    {
        private readonly HllPrecision _precision;
        private readonly int _numRegisters;
        private readonly HllRegister[] _registers;

        public HyperLogLog() : this(new HllPrecision(14)) { }
        public HyperLogLog(HllPrecision precision)
        {
            if (precision < 4 || precision > 16)
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be between 4 and 16");

            _precision = precision;
            _numRegisters = 1 << _precision;
            _registers = new HllRegister[_numRegisters];

            for (int i = 0; i < _numRegisters; i++)
                _registers[i] = new HllRegister();
        }

        public void Add(string item)
        {
            var hash = Hasher.Hash(item);
            var index = (int)(hash >> (64 - _precision));
            var w = hash << _precision;
            var leadingZeros = BitUtils.CountLeadingZeros(w) - _precision + 1;

            _registers[index].Update((byte)leadingZeros);
        }

        public double Estimate()
        {
            double alphaMM = GetAlphaMM();
            double sum = 0.0;

            foreach (var reg in _registers)
                sum += 1.0 / Math.Pow(2.0, reg.Value);

            double estimate = alphaMM / sum;

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
    }
}
