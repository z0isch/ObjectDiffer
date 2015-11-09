using System;

namespace ObjectDiffer.TypeDiffers
{
    // used for diffing primatives, and structs/objects that should be treated as primatives (e.g. strings)
    public class PrimativeDiffer : ITypeDiffer
    {
        public bool CanPerformDiff(Type t)
        {
            return t.IsPrimitive || t == typeof (string) || t == typeof(DateTime);
        }

        public Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback)
        {
            // "cast" both objects to dynamic so their actual types are determined at runtime, and the "==" operator works as expected
            return ((dynamic) newObj) == ((dynamic) oldObj) ? null : new Difference(propName, newObj, oldObj);
        }
    }
}
