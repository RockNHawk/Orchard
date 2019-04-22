using MnLab.PdfVisualDesign.Models;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;

namespace MnLab.PdfVisualDesign {
    public class PdfGeneratePart : ContentPart<PdfGeneratePartRecord> {
        public string FileName {
            get { return Retrieve(x => x.FileName); }
            set { Store(x => x.FileName, value); }
        }
        public DateTime CreatedUtc {
            get { return Retrieve(x => x.CreatedUtc); }
            set { Store(x => x.CreatedUtc, value); }
        }
    }
}