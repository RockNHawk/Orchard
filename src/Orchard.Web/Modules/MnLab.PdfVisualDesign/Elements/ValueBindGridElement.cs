using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;

namespace MnLab.PdfVisualDesign.Binding.Elements {

    public class ValueBindGridViewModel {

        public IEnumerable<IGrouping<ContentPartDefinition, IValueBindingDef>> BindingDefSources { get; set; }
        public ValueBindGridData DesignData { get; set; }
        public Dictionary<string, object> ValueMaps { get; set; }
    }

    public class ValueBindGridData {
        public virtual ValueBindingDef[][] AllCellValues { get; set; }
        public virtual MergedCell[] MergedCells { get; set; }
    }

    public class MergedCell {
        public int row { get; set; }
        public int col { get; set; }
        //[Newtonsoft.Json.JsonProperty()]
        public int rowspan { get; set; }
        public int colspan { get; set; }
        public bool removed { get; set; }
    }

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
            get { return T("Value Bind Grid"); }
        }

        /// <summary>
        /// Binding jagged array data used for handsometable
        /// </summary>
        public virtual ValueBindGridData DesignData {
            get { return this.Retrieve(x => x.DesignData); }
            set { this.Store(x => x.DesignData, value); }
        }

        public virtual string Remark {
            get { return this.Retrieve(x => x.Remark); }
            set { this.Store(x => x.Remark, value); }
        }

    }

}