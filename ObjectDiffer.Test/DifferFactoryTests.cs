using System.Collections.Generic;
using NUnit.Framework;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer.Test
{
    [TestFixture]
    public class DifferFactoryTests
    {
        private DifferFactory _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new DifferFactory();
        }

        [Test]
        public void DefaultDifferShouldNotBeNull()
        {
            Assert.IsNotNull(_sut.GetDefault());
        }

        [Test]
        public void DifferWithTypeDiffersShouldNotBeNull()
        {
            Assert.IsNotNull(_sut.GetWithTypeDiffers(new List<ITypeDiffer>()));
        }
    }
}
