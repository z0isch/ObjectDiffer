using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ObjectDiffer.Test
{
    [TestFixture]
    public class DifferenceTests
    {
        [Test]
        public void IndexerShouldReturnCorrectChildDiffIfExists()
        {
            var expected = new Difference("a", 1337, 0);
            var difference = new Difference("prop", 1, 2)
            {
                ChildDiffs = new List<Difference>
                {
                    expected,
                    new Difference("b", "still nope", "nope")
                }
            };

            var child = difference["a"];
            Assert.AreSame(expected, child);
        }

        [Test]
        public void IndexerShouldReturnNullIfNoChildDiffExists()
        {
            var difference = new Difference("prop", 1, 2)
            {
                ChildDiffs = new List<Difference>
                {
                    new Difference("b", "still nope", "nope")
                }
            };

            var child = difference["a"];

            Assert.IsNull(child);
        }

        [Test]
        public void FilterShouldRemoveNonMatchingChildDiffs()
        {
            var difference = new Difference("prop", 1, 2)
            {
                ChildDiffs = new List<Difference>
                {
                    new Difference("a", 1337, 0),
                    new Difference("b", 123, 456)
                }
            };

            var filtered = difference.Filter(d => d.PropertyName != "a");

            Assert.AreEqual(1, filtered.ChildDiffs.Count());
            var childDiff = filtered.ChildDiffs.First();
            Assert.AreEqual("b", childDiff.PropertyName);
            Assert.AreEqual(123, childDiff.NewValue);
            Assert.AreEqual(456, childDiff.OldValue);
            Assert.IsNull(childDiff.ChildDiffs);
        }

        [Test]
        public void FilterShouldReturnNullIfNoMatchingChildDiffs()
        {
            var difference = new Difference("prop", 1, 2)
            {
                ChildDiffs = new List<Difference>
                {
                    new Difference("a", 1337, 0),
                }
            };

            var filtered = difference.Filter(d => false);

            Assert.IsNull(filtered);
        }
    }
}
