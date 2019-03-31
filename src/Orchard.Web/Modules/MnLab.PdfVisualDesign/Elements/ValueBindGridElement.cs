using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Utility;

namespace MnLab.PdfVisualDesign.Binding.Elements {


    public static class E2 {

        public static TValue RetrieveObject<TValue>(this Element element, string name) {
            var data = element.Data;
            var value = data.Get(name);
            return value == null ? default(TValue) : Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(value);
        }

        public static void StoreObject<TElement, TValue>(this TElement element, string name, TValue value) where TElement : Element {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            Store(element, name, json);
        }

        public static void Store(this Element element, string name, string value) {
            element.Data[name] = (value);
        }

    }


    public class ValueBindGridViewModel {

        public IEnumerable<IGrouping<ContentPartDefinition, IValueBindingDef>> BindingDefSources { get; set; }
        public ValueBindGridData DesignData { get; set; }
        public Dictionary<string, object> ValueMaps { get; set; }
    }

    public class ValueBindGridData : IValueBindGridData {
        public virtual ValueBindingDef[][] AllCellValues { get; set; }
        public virtual MergedCell[] MergedCells { get; set; }
        /// <summary>
        /// Table / UI/LI List
        /// </summary>
        public virtual string GridType { get; set; }
        public virtual string DisplayType { get; set; }
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
    public class ValueBindGridElement : ContentElement //,IValueBindGridData
        {
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
            get { return this.RetrieveObject<ValueBindGridData>(nameof(DesignData)); }
            set { this.StoreObject(nameof(DesignData), value); }
        }

        /// <summary>
        /// Table / UI/LI List
        /// </summary>
        public virtual string GridType {
            get { return this.Retrieve(x=>x.GridType); }
            set { this.Store(x=>x.GridType, value); }
        }

        public virtual string DisplayType {
            get { return this.Retrieve(x => x.DisplayType); }
            set { this.Store(x => x.DisplayType, value); }
        }

        public virtual string Remark {
            get { return this.Retrieve(x => x.Remark); }
            set { this.Store(x => x.Remark, value); }
        }

    }

}