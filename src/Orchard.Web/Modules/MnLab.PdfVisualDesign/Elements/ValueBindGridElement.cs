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
    /// ���� Model Property Bind Element
    /// ��������ģʽ��
    /// ���ӻ��༭�󶨹���ģʽ
    /// ���ӻ��������հ󶨵�����ģʽ����������ǰ���������ݱ��浽 content ��Ӧ�� ContentPart �У�
    /// ͨ�� Tab ������ģʽͬʱչ�ֵ�ģʽ
    ///
    /// �� Orchard ���й��ܼܹ�������չ��Ӧ�ÿ���
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