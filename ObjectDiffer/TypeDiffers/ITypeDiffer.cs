using System;

namespace ObjectDiffer.TypeDiffers
{
    public interface ITypeDiffer
    {
        bool CanPerformDiff(Type t);
        Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback);
    }
}
