using Orchard.ContentManagement;
using System.Collections.Generic;

namespace MnLab.PdfVisualDesign.Fields
{
    public class TempalteSupportField : ContentField
    {
        public string HTML
        {
            get
            {
                return Storage.Get<string>("HTML");
            }
            set
            {
                Storage.Set<string>("HTML", value);
            }
        }
    }
}