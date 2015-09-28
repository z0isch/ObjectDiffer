using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using NSubstitute;
using NUnit.Framework;
using ObjectDiffer.TypeDiffers;

namespace ObjectDiffer.Test
{
    public abstract class DifferTests
    {
        private IDiffer _differ;

        protected abstract IDiffer GetDiffer();

        [SetUp]
        public void Setup()
        {
            _differ = GetDiffer();
        }

        private class TestObj
        {
            public int Id { get; set; }
            public string Primative { get; set; }
            public TestObj Object { get; set; }
            public List<TestObj> List { get; set; }

            public override bool Equals(object obj)
            {
                var testObj = obj as TestObj;
                if (testObj == null)
                    return false;
                return testObj.Id == Id;
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        [Test]
        public void PropertyDifferenceIndexerShouldReturnChildDiffWithMatchingPropertyName()
        {
            var expected = new Difference("prop", "a", "b");
            var diff = new Difference("test", "a", "b")
            {
                ChildDiffs = new List<Difference>
                {
                    expected
                }
            };
            Assert.AreEqual(diff["prop"], expected);
        }

        [Test]
        public void RootDiffObjectShouldShowDifferencesInRootObject()
        {
            var diff = _differ.Diff("A", "B");
            Assert.AreEqual("self", diff.PropertyName);
            Assert.AreEqual("A", diff.NewValue);
            Assert.AreEqual("B", diff.OldValue);
        }

        [Test]
        public void ShouldReturnNullWhenNoDifference()
        {
            var diff = _differ.Diff("A", "A");
            Assert.IsNull(diff);
        }

        [Test]
        public void DiffShouldHighlightChangedPrimativeProperties()
        {
            var oldObj = new TestObj
            {
                Primative = "A"
            };
            var newObj = new TestObj
            {
                Primative = "B"
            };
            var diff = _differ.Diff(newObj, oldObj);
            Assert.AreEqual(1, diff.ChildDiffs.Count());
            Assert.AreEqual("A", diff.ChildDiffs.First().OldValue);
            Assert.AreEqual("B", diff.ChildDiffs.First().NewValue);
            Assert.AreEqual("Primative", diff.ChildDiffs.First().PropertyName);
        }

        [Test]
        public void DiffShouldReturnChangedChildProperties()
        {
            var old = new TestObj
            {
                Object = new TestObj
                {
                    Id = 1,
                    Primative = "Val1"
                }
            };
            var newObj = new TestObj
            {
                Object = new TestObj
                {
                    Id = 1,
                    Primative = "Val2"
                }
            };

            var diff = _differ.Diff(newObj, old);
            Assert.AreEqual(1, diff.ChildDiffs.Count());
            Assert.AreEqual(1, diff.ChildDiffs.First().ChildDiffs.Count());
            Assert.AreEqual("Val2", diff.ChildDiffs.First().ChildDiffs.First().NewValue);
            Assert.AreEqual("Primative", diff.ChildDiffs.First().ChildDiffs.First().PropertyName);
        }

        [Test]
        public void DiffShouldReturnAddedArrayElement()
        {
            var old = new List<string> {"a"};
            var newList = new List<string> {"a", "b"};
            var diff = _differ.Diff(newList, old);
            Assert.AreEqual(1, diff.ChildDiffs.Count());
            Assert.AreEqual("b", diff.ChildDiffs.First().NewValue);
            Assert.AreEqual(null, diff.ChildDiffs.First().OldValue);
            Assert.AreEqual("Item", diff.ChildDiffs.First().PropertyName);
        }

        [Test]
        public void DiffForObjectArrayShouldReturnChangedObjects()
        {
            var oldChildObj = new TestObj
            {
                Id = 1,
                Primative = "A"
            };
            var old = new List<TestObj>
            {
                oldChildObj
            };
            var newChildObj = new TestObj
            {
                Id = 1,
                Primative = "B"
            };
            var newObj = new List<TestObj>
            {
                newChildObj
            };
            var diff = _differ.Diff(newObj, old);
            Assert.AreEqual(1, diff.ChildDiffs.Count());
            Assert.IsTrue(diff.ChildDiffs.Any(d => d.NewValue == newChildObj && d.OldValue == oldChildObj));
        }

        [Test]
        public void ShouldDiffArrayPropertiesOfObjects()
        {
            var addedObj = new TestObj
            {
                Id = 3,
                Primative = "str3"
            };
            var newObj = new TestObj
            {
                Id = 1,
                List = new List<TestObj>
                {
                    new TestObj
                    {
                        Id = 2,
                        Primative = "str1"
                    },
                    addedObj
                }
            };
            var oldObj = new TestObj
            {
                Id = 1,
                List = new List<TestObj>
                {
                    new TestObj
                    {
                        Id = 2,
                        Primative = "str2"
                    }
                }
            };

            var diff = _differ.Diff(newObj, oldObj);
            Assert.IsNotNull(diff);
            Assert.AreEqual(1, diff.ChildDiffs.Count());

            var changedObjDiff = diff["List"].ChildDiffs.First(d => d.NewValue != null && d.OldValue != null);
            Assert.AreEqual(1, changedObjDiff.ChildDiffs.Count());
            Assert.AreEqual("str1", changedObjDiff.ChildDiffs.First().NewValue);
            Assert.AreEqual("str2", changedObjDiff.ChildDiffs.First().OldValue);

            var addedObjDiff = diff["List"].ChildDiffs.First(d => d.NewValue != null && d.OldValue == null);
            Assert.AreEqual(addedObj, addedObjDiff.NewValue);
            Assert.AreEqual(null, addedObjDiff.OldValue);
        }

        [Test]
        public void ShouldDiffArraysWhenNull()
        {
            List<string> old = null;
            var newList = new List<string> {"a"};

            var diff = _differ.Diff(newList, old);
            Assert.IsNotNull(diff);
            Assert.AreEqual(newList, diff.NewValue);
            Assert.AreEqual(null, diff.OldValue);
            Assert.AreEqual(1, diff.ChildDiffs.Count());
            Assert.AreEqual("a", diff.ChildDiffs.First().NewValue);
            Assert.AreEqual(null, diff.ChildDiffs.First().OldValue);
        }

        [Test]
        public void DiffShouldIgnoreUnchangedProperties()
        {
            var oldObj = new TestObj
            {
                Primative = "A",
                Object = new TestObj()
            };
            var newObj = new TestObj
            {
                Primative = "A",
                Object = new TestObj()
            };
            var diff = _differ.Diff(newObj, oldObj);
            Assert.IsNull(diff);
        }

        [Test]
        public void ShouldBePerformantForObjectDiffs()
        {
            var oldObj = new TestObj
            {
                Id = 1,
                Primative = "A",
                Object = new TestObj
                {
                    Primative = "A"
                }
            };
            var newObj = new TestObj
            {
                Id = 1,
                Primative = "B",
                Object = new TestObj
                {
                    Primative = "B"
                }
            };

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Enumerable.Range(0, 1000).Select(i =>
            {
                oldObj.Primative = i.ToString();
                return _differ.Diff(newObj, oldObj);
            }).ToList();
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 1000);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void ShouldBePerformantForArrayDiffs()
        {
            var oldList = Enumerable.Range(0, 50);
            var newList = Enumerable.Range(0, 50);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Enumerable.Range(0, 100).Select(i =>
            {
                newList = newList.Select(e => e + 1);
                return _differ.Diff(newList, oldList);
            }).ToList();
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 1000);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void ShouldPerformDiffOnArrayOfPrimatives()
        {
            // The differ doesn't know the 3 in the old list and the 4 in the new list are supposed to be the same.
            // Should return 2 child diffs, one for adding the 4 and one for removing the 3
            var result = _differ.Diff(new List<int> {1, 2, 4}, new List<int> {1, 2, 3});
            Assert.IsTrue(result.ChildDiffs.Any(d => d.NewValue is int && (int) d.NewValue == 4 && d.OldValue == null));
            Assert.IsTrue(result.ChildDiffs.Any(d => d.OldValue is int && (int) d.OldValue == 3 && d.NewValue == null));
        }
    }

    [TestFixture]
    public class NinjectDifferTests : DifferTests
    {
        protected override IDiffer GetDiffer()
        {
            var kernel = new StandardKernel(new ObjectDifferModule());
            return kernel.Get<IDiffer>();
        }
    }

    [TestFixture]
    public class FactoryDifferTests : DifferTests
    {
        protected override IDiffer GetDiffer()
        {
            return new DifferFactory().GetDefault();
        }
    }

    [TestFixture]
    public class ManuallyCreatedDifferTests : DifferTests
    {
        protected override IDiffer GetDiffer()
        {
            return new Differ(
                new List<ITypeDiffer>
                {
                    new PrimativeDiffer(),
                    new ObjectEqualityEnumerableDiffer(new DefaultEqualityComparer()),
                    new ObjectTypeDiffer()
                });
        }
    }
}
