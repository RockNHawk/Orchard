using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using ICTranslator.Models;

namespace ICTranslator.Drivers
{
    public class ICTranslatorDriver : ContentPartDriver<ICTranslatorPart>
    {
        protected override DriverResult Display(ICTranslatorPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_ICTranslator",
                                () => shapeHelper.Parts_ICTranslator(
                                    ToLanguage: part.ToLanguage,
                                    FromLanguage: part.FromLanguage,
                                    TranslateText: part.TranslateText
                                    ));
        }

        //GET
        protected override DriverResult Editor(ICTranslatorPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_ICTranslator_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: "Parts/ICTranslator", Model: part, Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(ICTranslatorPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
