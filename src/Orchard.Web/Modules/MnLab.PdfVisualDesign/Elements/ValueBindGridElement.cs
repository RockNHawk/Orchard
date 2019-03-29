using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;

namespace MnLab.PdfVisualDesign.Binding.Elements {

  
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