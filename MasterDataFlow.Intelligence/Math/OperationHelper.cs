using System;
using System.Collections.Generic;

namespace MasterDataFlow.Intelligence.Math
{
    public static class OperationHelper
    {
        private static readonly Dictionary<Type, object> provider = new Dictionary<Type, object>();

        static OperationHelper()
        {
            provider[typeof(int)] = Int32OperationProvider.Instance;
            provider[typeof(float)] = FloatOperationProvider.Instance;
        }

        public static void SetProvider<T>(OperationProvider<T> Provider)
            where T : IComparable<T>, IEquatable<T>
        {
            provider[typeof(T)] = Provider;
        }

        public static OperationProvider<T> GetProvider<T>()
            where T : IComparable<T>, IEquatable<T>
        {
            var type = typeof(T);

            if (!provider.ContainsKey(type))
                throw new InvalidOperationException(type.Name + " is not on the list");
            return provider[type] as OperationProvider<T>;
        }
    }
}