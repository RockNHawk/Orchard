using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;

namespace MnLab.PdfVisualDesign {

    public class ValueBindingDef : IValueBindingDef {
        public string ContentPartName { get; set; }
        public string MemberExpression { get; set; }
        public string DefaultValue { get; set; }
        /// <summary>
        /// 保存直接输入值，不绑定时的值，此时 BindType 为 "StaticValue"，其它字段为空
        /// 在 ValueBindingDef 添加此字段是为了服务端便于统一存储 Cell Value 的 JSON 为 ValueBindingDef 类型，但是客户端就需要做额外的处理了
        /// 不过也好，这样便于扩展
        /// </summary>
        public string StaticValue { get; set; }
        public LocalizedString Description { get; set; }
        public string Remark { get; set; }
        public string BindType { get; set; }
        public string Key => string.IsNullOrEmpty(ContentPartName)|| string.IsNullOrEmpty(MemberExpression)?null: ContentPartName + "." + MemberExpression;
    }
}