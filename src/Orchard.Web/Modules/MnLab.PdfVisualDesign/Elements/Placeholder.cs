using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;

namespace MnLab.PdfVisualDesign.Elements {
    /// <summary>
    /// 
    /// </summary>
    public class Placeholder : Container {

        public override string ToolboxIcon {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText {
            get { return T("Placeholder"); }
        }
        public override string Category {
            get { return "Layout"; }
        }

        public override bool HasEditor => false;

        public virtual string PlaceId {
            get { return this.Retrieve(x => x.PlaceId); }
            set { this.Store(x => x.PlaceId, value); }
        }

    }
}