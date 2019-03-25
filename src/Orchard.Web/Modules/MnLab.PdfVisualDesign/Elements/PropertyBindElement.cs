using Orchard.Layouts.Elements;
using Orchard.Localization;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;

namespace MnLab.PdfVisualDesign.HtmlBlocks.Elements 
{
    /// <summary>
    /// ���� Model Property Bind Element
    /// ��������ģʽ��
    /// ���ӻ��༭�󶨹���ģʽ
    /// ���ӻ��������հ󶨵�����ģʽ����������ǰ���������ݱ��浽 content ��Ӧ�� ContentPart �У�
    /// ͨ�� Tab ������ģʽͬʱչ�ֵ�ģʽ
    ///
    /// �� Orchard ���й��ܼܹ�������չ��Ӧ�ÿ���
    /// </summary>
    public class PropertyBindElement : ContentElement 
    {
        public override string ToolboxIcon
        {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText
        {
            get { return T("Property Bind"); }
        }

        public virtual string PropertyExpression {
            get { return this.Retrieve(x => x.PropertyExpression); }
            set { this.Store(x => x.PropertyExpression, value); }
        }



    }

}