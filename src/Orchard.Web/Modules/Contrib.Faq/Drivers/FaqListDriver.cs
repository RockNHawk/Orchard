using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Routable.Models;
using Orchard.Environment.Extensions;
using Contrib.Faq.Models;

namespace Contrib.Faq.Drivers {
  public class FaqListDriver : ContentPartDriver<FaqList> {
	private readonly IContentManager _cms;

	public FaqListDriver(IContentManager cms)
	{
		 _cms = cms;
	} 	
    
    protected override DriverResult Display(
        FaqList flist, string displayType, dynamic shapeHelper)
    {
		// If we have no category.....
		if (String.IsNullOrWhiteSpace(flist.Category))
		{
			return ContentShape("Faq_List",
				() => shapeHelper.Parts_FaqList(
				ContentItems: shapeHelper.List(),
				Category: flist.Category
			));
		}
		
		IEnumerable<FaqPart> parts = 
			_cms.Query<FaqPart, FaqPartRecord>()
			.Where(fpr => fpr.Category.Contains(flist.Category))
			.OrderByDescending(fpr => fpr.SubCategory)
			.Join<CommonPartRecord>()			
			.List();

		var xparts = parts.OrderBy(part => part.SubCategory).ThenBy(part => part.Question);
			
		return ContentShape("Faq_List",
			() => shapeHelper.Parts_FaqList(
			Intro : flist.Intro,
			Category : flist.Category,
			Faqs: xparts
		));			
    }

    //GET
    protected override DriverResult Editor(FaqList flist, dynamic shapeHelper)
    {
      return ContentShape("Faq_List_Edit",
          () => shapeHelper.EditorTemplate(
              TemplateName: "Parts/FaqList",
              Model: flist,
              Prefix: Prefix));
    }
 
    //POST
    protected override DriverResult Editor(
        FaqList flist, IUpdateModel updater, dynamic shapeHelper)
    {
      updater.TryUpdateModel(flist, Prefix, null, null);
      return Editor(flist, shapeHelper);
    }
  }
}
