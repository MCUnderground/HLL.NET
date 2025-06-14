
namespace HLL.NET.Hashing
{
    public interface IHasher<in T>
    {
        ulong Hash(T value);
    }

}
