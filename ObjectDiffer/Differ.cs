using System;
using System.Collections.Generic;
using System.Linq;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class Differ : IDiffer
    {
        private readonly IEnumerable<ITypeDiffer> _typeDiffers;

        // type differs should be ordered by how specific they are - 
        // e.g. the object type differ should be after the primative type differ
        public Differ(IEnumerable<ITypeDiffer> typeDiffers)
        {
            _typeDiffers = typeDiffers;
        }

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

            // get the first type differ where CanPerformDiff is true, then perform the diff
            return _typeDiffers.First(d => d.CanPerformDiff(objectType)).PerformDiff(newObj, oldObj, propName, objectType, GetDifferences);
        }

        // just a wrapper for the GetDifferences method to make the public API a bit nicer
        public Difference Diff<T>(T newObj, T oldObj)
        {
            return GetDifferences(newObj, oldObj, "self", typeof(T));
        }
    }
}
