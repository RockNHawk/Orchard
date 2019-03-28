using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Contrib.Faq.Models {

  public class FaqPartRecord : ContentPartRecord {
	[StringLengthMax]
    public virtual string Question { get; set; }
	[StringLengthMax]
    public virtual string Answer { get; set; }
	public virtual string Category { get; set; }
	public virtual string SubCategory { get; set; }
  }

  public class FaqPart : ContentPart<FaqPartRecord> {
    [Required]
    public string Question {
      get { return Record.Question; }
      set { Record.Question = value; }
    }

    [Required]
    public string Answer {
      get { return Record.Answer; }
      set { Record.Answer = value; }
    }

    [Required]
    public string Category {
      get { return Record.Category; }
      set { Record.Category = value; }
    }

    [Required]
    public string SubCategory {
      get { return Record.SubCategory; }
      set { Record.SubCategory = value; }
    }
  }
  
	public class ListFaqPartRecord : FaqPart {
		public ContentItem contentItem { get; set; }
	}
}
