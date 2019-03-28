using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I9AOrchard.MediaPickerField.Settings
{
    public class MediaPickerFieldSettings
    {
        public const string MediaPathDefaultDefault = "Images";
        private string _mediapathDefault;
        public string MediaPathDefault
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_mediapathDefault)
                         ? _mediapathDefault
                         : MediaPathDefaultDefault;
            }
            set { _mediapathDefault = value; }
        }
    }
}