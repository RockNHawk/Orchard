using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace ICTranslator.Models {
    public class ICTranslatorRecord: ContentPartRecord {
        [StringLength(20)]
        public virtual string FromLanguage { get; set; }

        [StringLength(20)]
        public virtual string ToLanguage { get; set; }

        [StringLengthMax]
        public virtual string TranslateText { get; set; }
    }

    public class ICTranslatorPart : ContentPart<ICTranslatorRecord>
    {

        public string TranslateText
        {
            get { return ((Record.TranslateText == null) ? "" : Record.TranslateText); }
            set { Record.TranslateText = value; }
        }

        [Required]
        public string FromLanguage {
            get { return Record.FromLanguage; }
            set { Record.FromLanguage = value; }
        }

        [Required]
        public string ToLanguage {
            get { return Record.ToLanguage; }
            set { Record.ToLanguage = value; }
        }
    }
}
