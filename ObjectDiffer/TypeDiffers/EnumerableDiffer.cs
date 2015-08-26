using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ninject;

namespace ObjectDiffer.TypeDiffers
{
    public class EnumerableDiffer : ITypeDiffer
    {
        private readonly IEqualityComparer<object> _sameObjectComparer;

        public EnumerableDiffer([Named("SameObjectComparer")]IEqualityComparer<object> sameObjectComparer)
        {
            _sameObjectComparer = sameObjectComparer;
        }

        public bool CanPerformDiff(Type t)
        {
            return typeof (IEnumerable).IsAssignableFrom(t) && t.IsGenericType;
        }

        public Difference PerformDiff(object newObj, object oldObj, string propName, Type type, Func<object, object, string, Type, Difference> diffChildCallback)
        {
            var newArray = newObj == null ? new List<object>() : (newObj as IEnumerable).Cast<object>().ToList();
            var oldArray = oldObj == null ? new List<object>() : (oldObj as IEnumerable).Cast<object>().ToList();

            var parentDifference = new Difference(propName, newObj, oldObj)
            {
                ChildDiffs = newArray.Union(oldArray).Distinct(_sameObjectComparer)
                    .Select(elem => new
                    {
                        newItem = newArray.FirstOrDefault(x => _sameObjectComparer.Equals(x, elem)),
                        oldItem = oldArray.FirstOrDefault(x => _sameObjectComparer.Equals(x, elem))
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
