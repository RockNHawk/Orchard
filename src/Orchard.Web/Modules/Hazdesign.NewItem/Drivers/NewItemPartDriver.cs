using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Hazdesign.NewItem.Models;
namespace Hazdesign.NewItem.Drivers {
    public class NewItemPartDriver : ContentPartDriver<NewItemPart> {
        private readonly IOrchardServices _services;
        public NewItemPartDriver(IOrchardServices services){
            _services = services;
        }
        protected override DriverResult Display(NewItemPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_NewItem",
                    () => {
                        var settings = this._services.WorkContext.CurrentSite.As<NewItemSettingsPart>();
                        var commonPart = part.ContentItem.As<CommonPart>();
                        var isNewItem = commonPart.PublishedUtc.GetValueOrDefault().Date >= DateTime.Today.AddDays(-settings.Days);
                        return shapeHelper.Parts_NewItem(IsNewItem: isNewItem);
                    });
        }
    }
}