using System.Collections.Generic;

namespace ObjectDiffer
{
    public class HashCodeEqualityComparer : IEqualityComparer<object>
    {
        public bool Equals(object x, object y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
