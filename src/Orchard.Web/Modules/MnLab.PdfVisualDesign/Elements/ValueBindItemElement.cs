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
    public class PropertyBindElement : ContentElement, IValueBindingDef {
        public override string ToolboxIcon {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText {
            get { return T("Value Bind Item"); }
        }

        public virtual string MemberExpression {
            get { return this.Retrieve(x => x.MemberExpression); }
            set { this.Store(x => x.MemberExpression, value); }
        }

        public virtual string ContentPartMemberExpression {
            get { return MemberExpression; }
            set { MemberExpression = value; }
        }

        public virtual string ContentPartName {
            get { return this.Retrieve(x => x.ContentPartName); }
            set { this.Store(x => x.ContentPartName, value); }
        }

        public virtual string DefaultValue {
            get { return this.Retrieve(x => x.DefaultValue); }
            set { this.Store(x => x.DefaultValue, value); }
        }

        public virtual string Remark {
            get { return this.Retrieve(x => x.Remark); }
            set { this.Store(x => x.Remark, value); }
        }

        public string Key => ContentPartName + "." + MemberExpression;

        //LocalizedString IValueBindingInfo.Description { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }

}