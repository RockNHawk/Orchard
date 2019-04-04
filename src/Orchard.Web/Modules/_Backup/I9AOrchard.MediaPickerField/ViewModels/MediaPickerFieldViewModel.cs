using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I9AOrchard.MediaPickerField.ViewModels
{
    public class MediaPickerFieldViewModel
    {
        public string Name { get; set; }
        public Fields.MediaPickerField MediaPickerField { get; set; }
        public string PickerPath { get { return MediaPickerField.PickerPath; } set { MediaPickerField.PickerPath = value; } }
        public string EditorMediaPath { get; set; }
    }
}