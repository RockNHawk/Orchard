using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Orchard.Environment.Extensions;


namespace Guang.HiddenField.Fields
{
    
    public class GuangHiddenField: ContentField 
    {
        public string Value
        {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }
    }
}



