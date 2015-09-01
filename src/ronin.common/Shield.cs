using System;

namespace Ronin.Common
{
    public static class Shield
    {
        public static void EnsureNotNull<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "The given object cannot be null");
        }

        public static void EnsureNotNullOrEmpty(string obj)
        {
            if (string.IsNullOrEmpty(obj))
                throw new ArgumentException(nameof(obj), "The given string literal cannot be null or empty");
        }

        public static void EnsureGreaterThan(int actual, int reference)
        {
            if (reference >= actual)
                throw new ArgumentException(string.Format("The given number is lesser or equal than: {0}", reference));
        }

        public static void EnsureTrue(bool condition)
        {
            if (!condition)
                throw new ArgumentException();
        }

        public static void EnsureTypeOf<TType>(object obj)
        {
            if (!(obj is TType))
                throw new ArgumentException(nameof(obj));
        }
    }
}
