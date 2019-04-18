using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentPicker.Models;
using Orchard.Core.Navigation.Models;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;

namespace Orchard.ContentExtensions.Handlers
{
    //[UsedImplicitly]
    public class ContentExtensionMenuItemPartHandler: ContentHandler {
        private readonly IContentManager contentManager;

        public ContentExtensionMenuItemPartHandler(
            IContentManager contentManager, 
            IRepository<ContentMenuItemPartRecord> repository)
        {
            this.contentManager = contentManager;

            this.OnLoading<ContentMenuItemPart>((context, part) => part._content.Loader(p =>
            {
                if (part.Record.ContentMenuItemRecord != null) {
                    return contentManager.Get(part.Record.ContentMenuItemRecord.Id);
                }

                return null;
            }));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            base.GetItemMetadata(context);

            if (context.ContentItem.ContentType != "ContentExtensionMenuItem")
            {
                return;
            }

            var contentMenuItemPart = context.ContentItem.As<ContentMenuItemPart>();
            // the display route for the menu item is the one for the referenced content item
            if(contentMenuItemPart != null) {

                // if the content doesn't exist anymore
                if(contentMenuItemPart.Content == null) {
                    return;
                }

                context.Metadata.DisplayRouteValues["area"] = "Orchard.ContentExtensions";
                context.Metadata.DisplayRouteValues["controller"] = "Item";
                context.Metadata.DisplayRouteValues["type"] = contentMenuItemPart.Content.ContentType;
                context.Metadata.DisplayRouteValues["action"] = "Display";
                context.Metadata.DisplayRouteValues["id"] = contentMenuItemPart.Content.Id;
            }
        }
    }
}