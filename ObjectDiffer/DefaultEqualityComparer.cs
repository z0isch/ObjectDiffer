using System;
using System.Collections.Generic;

namespace ObjectDiffer
{
    public class DefaultEqualityComparer : IEqualityComparer<object>
    {
        public bool Equals(object x, object y)
        {
            return Object.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
