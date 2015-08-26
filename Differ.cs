using System;
using System.Collections.Generic;
using System.Linq;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class Differ : IDiffer
    {
        private static readonly IEnumerable<ITypeDiffer> TypeDiffers = new List<ITypeDiffer>
        {
            new PrimativeDiffer(),
            new EnumerableDiffer(),
            new TypeDiffers.ObjectDiffer()
        };
        private object DefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        private Difference GetDifferences(object newObj, object oldObj, string propName, Type objectType)
        {
            // Return null straight away if both objects are the default for their type.
            // This prevents infinite recursion when a class has a property with the type of the parent class.
            if (Equals(newObj, DefaultValue(objectType)) && Equals(oldObj, DefaultValue(objectType)))
            {
                return null;
            }

            return TypeDiffers.First(d => d.CanPerformDiff(objectType)).PerformDiff(newObj, oldObj, propName, objectType, GetDifferences);
        }

        public Difference Diff<T>(T newObj, T oldObj)
        {
            return GetDifferences(newObj, oldObj, "self", typeof(T));
        }
    }
}
