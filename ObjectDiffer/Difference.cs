using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ObjectDiffer
{
    public class Difference
    {
        public string PropertyName { get; private set; }
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }
        public IEnumerable<Difference> ChildDiffs { get; set; }

        public Difference(string propName, object newVal, object oldVal)
        {
            this.PropertyName = propName;
            this.NewValue = newVal;
            this.OldValue = oldVal;
        }

        public Difference this[string name]
        {
            get { return this.ChildDiffs.FirstOrDefault(d => d.PropertyName == name); }
        }

        public Difference Filter(Predicate<Difference> filter)
        {
            if (!filter(this)) return null;

            var diff = new Difference(this.PropertyName, this.NewValue, this.OldValue)
            {
                ChildDiffs = this.ChildDiffs?.Select(d => d.Filter(filter)).Where(d => d != null)
            };
            return diff.ChildDiffs == null || diff.ChildDiffs.Any() ? diff : null;
        }
    }
}
