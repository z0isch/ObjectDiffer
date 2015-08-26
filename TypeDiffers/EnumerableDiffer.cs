using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDiffer.TypeDiffers
{
    public class EnumerableDiffer : ITypeDiffer
    {
        public bool CanPerformDiff(Type t)
        {
            return typeof (IEnumerable).IsAssignableFrom(t);
        }

        public Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback)
        {
            var newArray = newObj == null ? new List<object>() : (newObj as IEnumerable).Cast<object>().ToList();
            var oldArray = oldObj == null ? new List<object>() : (oldObj as IEnumerable).Cast<object>().ToList();
            var sameObjectComparer = new HashCodeEqualityComparer();
            var parentDifference = new Difference(propName, newObj, oldObj)
            {
                ChildDiffs = newArray.Union(oldArray).Distinct(sameObjectComparer)
                    .Select(elem => new
                    {
                        newItem = newArray.FirstOrDefault(x => sameObjectComparer.Equals(x, elem)),
                        oldItem = oldArray.FirstOrDefault(x => sameObjectComparer.Equals(x, elem))
                    })
                    .Select(
                        change =>
                            diffChildCallback(change.newItem, change.oldItem, "Item", GetEnumerableElementType(type)))
                    .Where(d => d != null)
                    .ToList()
            };
            // assume arrays are the same if there are no child differences
            return parentDifference.ChildDiffs.Any() ? parentDifference : null;
        }

        private Type GetEnumerableElementType(Type enumerableType)
        {
            return enumerableType.GetGenericArguments().First();
        }
    }
}
