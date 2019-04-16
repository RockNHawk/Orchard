using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Core.Contents.Extensions;
 
 namespace Orchard.ContentExtensions
 {
    public class Migrations : DataMigrationImpl
     {
        public int Create()
        {
            // Special menu that its items points to the ContentExtension Controllers
            ContentDefinitionManager.AlterTypeDefinition("ContentExtensionMenuItem", cfg => cfg
                    .WithPart("MenuPart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("ContentMenuItemPart")
                    .DisplayedAs("ContentExtension Menu Item")
                    .WithSetting("Description", "Adds a Content Item to the menu.")
                    .WithSetting("Stereotype", "MenuItem")
                     );

            return 1;
        }

        public int UpdateFrom1()
        {
            return 2;
        }
    }
}