using System;
using System.Collections.Generic;
using Ninject.Modules;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer
{
    public class ObjectDifferModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITypeDiffer>().To<PrimativeDiffer>();
            Bind<ITypeDiffer>().To<EnumerableDiffer>();

            Bind<ITypeDiffer>().To<ObjectTypeDiffer>();
            Bind<IEqualityComparer<object>>().To<HashCodeEqualityComparer>().Named("SameObjectComparer");

            Bind<IDiffer>().To<Differ>();
        }
    }
}
