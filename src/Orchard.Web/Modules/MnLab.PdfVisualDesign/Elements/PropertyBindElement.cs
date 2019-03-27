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
    public class PropertyBindElement : ContentElement {
        public override string ToolboxIcon {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText {
            get { return T("Property Bind"); }
        }

        public virtual string ContentPartMemberExpression {
            get { return this.Retrieve(x => x.ContentPartMemberExpression); }
            set { this.Store(x => x.ContentPartMemberExpression, value); }
        }

        public virtual string ContentPartName {
            get { return this.Retrieve(x => x.ContentPartName); }
            set { this.Store(x => x.ContentPartName, value); }
        }

        public virtual string ExampleValue {
            get { return this.Retrieve(x => x.ExampleValue); }
            set { this.Store(x => x.ExampleValue, value); }
        }

        public virtual string Remark {
            get { return this.Retrieve(x => x.Remark); }
            set { this.Store(x => x.Remark, value); }
        }

    }

}