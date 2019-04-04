using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Contrib.Faq.Models {

  public class FaqListRecord : ContentPartRecord {
	[StringLengthMax]
	public virtual string Intro { get; set; }
	public virtual string Category { get; set; }	
  }

  public class FaqList : ContentPart<FaqListRecord> {
    [Required]
    public string Intro {
      get { return Record.Intro; }
      set { Record.Intro = value; }
    }
	[Required]
    public string Category {
      get { return Record.Category; }
      set { Record.Category = value; }
    }
  }
}
