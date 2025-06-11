namespace HLL.NET.Models
{
    internal class HllRegister
    {
        public byte Value { get; private set; }

        public void Update(byte newValue)
        {
            if (newValue > Value)
                Value = newValue;
        }
    }
}