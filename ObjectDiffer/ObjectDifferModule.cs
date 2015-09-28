using System;
using System.Collections.Generic;
using Ninject.Modules;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class ObjectDifferModule : NinjectModule
    {
        private readonly bool _diffEnumerablesByIndex;

        public ObjectDifferModule(bool diffEnumerablesByIndex = false)
        {
            _diffEnumerablesByIndex = diffEnumerablesByIndex;
        }

        public override void Load()
        {
            Bind<ITypeDiffer>().To<PrimativeDiffer>();

            if (_diffEnumerablesByIndex)
            {
                Bind<ITypeDiffer>().To<IndexEnumerableDiffer>();
            }
            else
            {
                Bind<ITypeDiffer>().To<ObjectEqualityEnumerableDiffer>();
            }

            Bind<ITypeDiffer>().To<ObjectTypeDiffer>();
            Bind<IEqualityComparer<object>>().To<DefaultEqualityComparer>().Named("SameObjectComparer");

            Bind<IDiffer>().To<Differ>();
        }
    }
}
