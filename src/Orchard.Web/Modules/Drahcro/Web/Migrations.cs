using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using Drahcro;
using Drahcro.Web;
using Drahcro.Web.Models;

namespace Drahcro.Web {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            ContentDefinitionManager.AlterPartDefinition(nameof(WebCoreSupportPart), builder => builder
                .Attachable()
                .WithDescription("Provides web based core feature."));

            return 1;
        }

    }
}