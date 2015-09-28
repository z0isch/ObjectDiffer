using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ninject;

namespace ObjectDiffer.TypeDiffers
{
    public abstract class EnumerableDiffer : ITypeDiffer
    {
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
                ChildDiffs = GroupEqualObjects(newArray, oldArray)
                    .Select(
                        change =>
                            diffChildCallback(change.Item1, change.Item2, "Item", GetEnumerableElementType(type)))
                    .Where(d => d != null)
                    .ToList()
            };
            // assume arrays are the same if there are no child differences
            return parentDifference.ChildDiffs.Any() ? parentDifference : null;
        }

        protected abstract IEnumerable<Tuple<object, object>> GroupEqualObjects(IEnumerable<object> newArray, IEnumerable<object> oldArray);

        private Type GetEnumerableElementType(Type enumerableType)
        {
            return enumerableType.GetGenericArguments().First();
        }
    }
}
