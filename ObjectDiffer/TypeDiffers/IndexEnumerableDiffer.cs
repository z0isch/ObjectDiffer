using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiffer.TypeDiffers
{
    class IndexEnumerableDiffer : EnumerableDiffer
    {
        protected override IEnumerable<Tuple<object, object>> GroupEqualObjects(IEnumerable<object> newArray, IEnumerable<object> oldArray)
        {
            var maxLength = Math.Max(newArray.Count(), oldArray.Count());
            return Enumerable.Range(0, maxLength)
                .Select(i =>
                {
                    var newObj = i < newArray.Count() ? newArray.ElementAt(i) : null;
                    var oldObj = i < oldArray.Count() ? oldArray.ElementAt(i) : null;
                    return new Tuple<object, object>(newObj, oldObj);
                });
        }
    }
}
