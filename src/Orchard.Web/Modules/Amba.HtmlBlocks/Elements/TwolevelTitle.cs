//using Orchard.ContentManagement;
using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Helpers;
using Amba.HtmlBlocks.Models;
using Orchard.Layouts.Framework.Elements;

namespace Amba.HtmlBlocks.Binding.Elements 
{
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

    public class ValueTitleViewModel {
        /// <summary>
        /// 
        /// </summary>
        public ValueTitleData DesignData { get; set; }

    }
    public class ValueTitleData: IValueTitleData {
        /// <summary>
        /// User saved cell value expression object
        /// </summary>

        public virtual string MemberExpressions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string MemberExpression { get; set; }
    }

    public class TwolevelTitle : ContentElement {
        public override string ToolboxIcon
        {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText
        {
            get { return T("Twolevel Title"); }
        }
        public virtual ValueTitleData DesignData {
            get { return this.RetrieveObject<ValueTitleData>(nameof(DesignData)); }
            set { this.StoreObject(nameof(DesignData), value); }
        }
  
        public virtual string MemberExpression {
            get { return this.Retrieve(x => x.MemberExpression); }
            set { this.Store(x => x.MemberExpression, value); }
        }
        public virtual string MemberExpressions {
            get { return this.Retrieve(x => x.MemberExpressions); }
            set { this.Store(x => x.MemberExpressions, value); }
        }

        public virtual string ContentPartMemberExpression {
            get { return MemberExpressions; }
            set { MemberExpressions = value; }
        }
    }

}