using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace ObjectDiffer.TypeDiffers
{
    public class ObjectEqualityEnumerableDiffer : EnumerableDiffer
    {
        private readonly IEqualityComparer<object> _sameObjectComparer;

        public ObjectEqualityEnumerableDiffer([Named("SameObjectComparer")]IEqualityComparer<object> sameObjectComparer)
        {
            _sameObjectComparer = sameObjectComparer;
        }


        protected override IEnumerable<Tuple<object, object>> GroupEqualObjects(IEnumerable<object> newArray, IEnumerable<object> oldArray)
        {
            return newArray.Union(oldArray).Distinct(_sameObjectComparer)
                .Select(elem => new
                {
                    newItem = newArray.FirstOrDefault(x => _sameObjectComparer.Equals(x, elem)),
                    oldItem = oldArray.FirstOrDefault(x => _sameObjectComparer.Equals(x, elem))
                })
                .Select(x => new Tuple<object, object>(x.newItem, x.oldItem));
        }
    }
}
