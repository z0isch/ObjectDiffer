using System.Collections.Generic;
using Ninject;
using NSubstitute;
using NUnit.Framework;

namespace ObjectDiffer.Test
{
    [TestFixture]
    public class IoCTests
    {
        private IDiffer _differ;

        [SetUp]
        public void Setup()
        {
            var kernel = new StandardKernel(new ObjectDifferModule());
            _differ = kernel.Get<IDiffer>();
        }

        [Test]
        public void IDifferShouldBeBound()
        {
            Assert.IsNotNull(_differ);
        }

        [Test]
        public void ComparerToCheckIfObjectsShouldBeTheSameShouldBePluggable()
        {
            var comparer = Substitute.For<IEqualityComparer<object>>();
            var kernel = new StandardKernel(new ObjectDifferModule());
            kernel.Rebind<IEqualityComparer<object>>().ToConstant(comparer).Named("SameObjectComparer");
            var differ = kernel.Get<IDiffer>();

            var a = differ.Diff(new List<int> { 1, 2, 3 }, new List<int> { 2, 3, 4 });
            comparer.Received().Equals(Arg.Any<object>(), Arg.Any<object>());
        }
    }
}
