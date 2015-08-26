using System;
using System.Linq;
using System.Reflection;

namespace ObjectDiffer.TypeDiffers
{
    public class ObjectDiffer : ITypeDiffer
    {
        public bool CanPerformDiff(Type t)
        {
            return true;
        }

        public Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback)
        {
            var parentDifference = new Difference(propName, newObj, oldObj)
            {
                ChildDiffs = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(
                    p =>
                    {
                        var newValue = newObj == null ? null : p.GetValue(newObj, null);
                        var oldValue = oldObj == null ? null : p.GetValue(oldObj, null);
                        return diffChildCallback(newValue, oldValue, p.Name, p.PropertyType);
                    })
                    .Where(d => d != null)
                    .ToList()
            };
            // assume objects are the same if there are no child differences
            return parentDifference.ChildDiffs.Any() ? parentDifference : null;
        }
    }
}
