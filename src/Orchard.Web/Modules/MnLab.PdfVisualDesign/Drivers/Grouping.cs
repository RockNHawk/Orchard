using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace MnLab.PdfVisualDesign.Binding.Drivers {
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement> {

        IEnumerable<TElement> elements;
        public Grouping(TKey key, IEnumerable<TElement> elements) {
            this.Key = key;
            this.elements = elements;
        }
        public TKey Key { get; set; }

        public IEnumerator<TElement> GetEnumerator() {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}