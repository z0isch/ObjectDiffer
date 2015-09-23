using System.Collections.Generic;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class DifferFactory
    {
        public IDiffer GetDefault()
        {
            return new Differ(new List<ITypeDiffer>
            {
                new PrimativeDiffer(),
                new EnumerableDiffer(new HashCodeEqualityComparer()),
                new ObjectTypeDiffer()
            });
        }

        public IDiffer GetWithTypeDiffers(IEnumerable<ITypeDiffer> typeDiffers)
        {
            return new Differ(typeDiffers);
        }
    }
}
