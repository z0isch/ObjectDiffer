using System;

namespace ObjectDiffer.TypeDiffers
{
    public class PrimativeDiffer : ITypeDiffer
    {
        public bool CanPerformDiff(Type t)
        {
            return t.IsPrimitive || t == typeof (string) || t == typeof(DateTime);
        }

        public Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback)
        {
            return Equals(newObj, oldObj) ? null : new Difference(propName, newObj, oldObj);
        }
    }
}
