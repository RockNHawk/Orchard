using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;

namespace MnLab.PdfVisualDesign.Binding.Elements {

  
    /// <summary>
    /// 定义 Model Property Bind Element
    /// 具有三种模式：
    /// 可视化编辑绑定规则模式
    /// 可视化输入最终绑定的数据模式（这里最好是把输入的数据保存到 content 对应的 ContentPart 中）
    /// 通过 Tab 将两种模式同时展现的模式
    ///
    /// 对 Orchard 现有功能架构进行扩展，应该可行
    /// </summary>
    public class ValueBindGridElement : ContentElement {
        public override string ToolboxIcon {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText {
            get { return T("Property Bind"); }
        }

        /// <summary>
        /// Binding jagged array data used for handsometable
        /// </summary>
        public virtual FieldBindingInfo[][] BindingJaggeds {
            get { return this.Retrieve(x => x.BindingJaggeds); }
            set { this.Store(x => x.BindingJaggeds, value); }
        }

        public virtual string Remark {
            get { return this.Retrieve(x => x.Remark); }
            set { this.Store(x => x.Remark, value); }
        }

    }

}