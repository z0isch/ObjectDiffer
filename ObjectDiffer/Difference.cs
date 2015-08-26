using System.Collections.Generic;
using System.Linq;

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
    }
}
