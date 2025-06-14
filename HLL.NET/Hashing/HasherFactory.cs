using System;
namespace HLL.NET.Hashing
{
    public static class HasherFactory
    {
        public static IHasher<T> GetDefault<T>()
        {
            var t = typeof(T);

            if (t == typeof(string))
                return (IHasher<T>)new StringHasher();
            if (t == typeof(int))
                return (IHasher<T>)new IntHasher();
            if (t == typeof(Guid))
                return (IHasher<T>)new GuidHasher();

            throw new NotSupportedException($"No default hasher for type {typeof(T)}");
        }

    }

}
