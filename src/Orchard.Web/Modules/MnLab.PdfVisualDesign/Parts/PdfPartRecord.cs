using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace MnLab.PdfVisualDesign.Models {
    public class PdfGeneratePartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string FileName { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}
