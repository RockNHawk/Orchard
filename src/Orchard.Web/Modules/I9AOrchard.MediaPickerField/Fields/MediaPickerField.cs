using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Orchard.Environment.Extensions;

namespace I9AOrchard.MediaPickerField.Fields
{
    [OrchardFeature("MediaPickerField")]
    public class MediaPickerField : ContentField
    {
        public string PickerPath
        {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }
    }
}