using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace MnLab.Approval {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("ApprovalPartRecord",
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>("Title", column => column.WithLength(1024))
                );

            ContentDefinitionManager.AlterPartDefinition("ApprovalPart", builder => builder
                .Attachable()
                .WithDescription("Provides a Title for your content item."));

            return 2;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterPartDefinition("ApprovalPart", builder => builder
                .WithDescription("Provides a Title for your content item."));
            return 2;
        }
    }
}