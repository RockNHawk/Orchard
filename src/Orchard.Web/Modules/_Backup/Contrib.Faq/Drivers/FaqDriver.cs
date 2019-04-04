using Contrib.Faq.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;

namespace Contrib.Faq.Drivers {
  public class FaqDriver : ContentPartDriver<FaqPart> {
    protected override DriverResult Display(
        FaqPart part, string displayType, dynamic shapeHelper)
    {
      return ContentShape("Parts_Faq",
          () => shapeHelper.Parts_Faq(
              Question: part.Question,
              Answer: part.Answer,
			  Category: part.Category,
			  SubCategory: part.SubCategory));
		
    }

    //GET
    protected override DriverResult Editor(FaqPart part, dynamic shapeHelper)
    {
      return ContentShape("Parts_Faq_Edit",
          () => shapeHelper.EditorTemplate(
              TemplateName: "Parts/Faq",
              Model: part,
              Prefix: Prefix));
    }
 
    //POST
    protected override DriverResult Editor(
        FaqPart part, IUpdateModel updater, dynamic shapeHelper)
    {
      updater.TryUpdateModel(part, Prefix, null, null);
      return Editor(part, shapeHelper);
    }
  }
}
