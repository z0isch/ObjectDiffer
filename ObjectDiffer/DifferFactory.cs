using System.Collections.Generic;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class DifferFactory
    {
        public IDiffer GetDefault(bool diffEnumerablesByIndex = false)
        {
            return new Differ(new List<ITypeDiffer>
            {
                new PrimativeDiffer(),
                diffEnumerablesByIndex ? (ITypeDiffer)new IndexEnumerableDiffer() : new ObjectEqualityEnumerableDiffer(new DefaultEqualityComparer()),
                new ObjectTypeDiffer()
            });
        }

        public IDiffer GetWithTypeDiffers(IEnumerable<ITypeDiffer> typeDiffers)
        {
            return new Differ(typeDiffers);
        }
    }
}
