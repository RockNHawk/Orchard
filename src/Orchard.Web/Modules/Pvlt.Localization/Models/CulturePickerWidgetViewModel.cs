using System.Collections.Generic;
using System.Globalization;

namespace Pvlt.Localization.Models {
    public class CulturePickerWidgetViewModel {
        public CultureInfo CurrentCulture { get; set; }

        public IList<CulturePickerLink> Links { get; set; }
    }

    public class CulturePickerLink {
        public string Url { get; set; }

        public CultureInfo Culture { get; set; }
    }
}
