namespace HLL.NET.Models
{
    internal class HllRegister
    {
        public byte Value { get; private set; }

        public HllRegister(byte initialValue = 0)
        {
            Value = initialValue;
        }

        public void Update(byte newValue)
        {
            if (newValue > 64) newValue = 64;
            if (newValue > Value) Value = newValue;
        }
    }
}