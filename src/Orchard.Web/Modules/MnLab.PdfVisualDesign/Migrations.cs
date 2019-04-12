using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace MnLab.PdfVisualDesign.HtmlBlocks {
    public class Migrations : DataMigrationImpl {
        public int Create() {

            SchemaBuilder.CreateTable(nameof(TempalteSupportPartRecord),
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>(nameof(TempalteSupportPartRecord.Title), column => column.WithLength(1024))
                );

            ContentDefinitionManager.AlterPartDefinition(nameof(TempalteSupportPart), builder => builder
                .Attachable()
                .WithDescription("Provides a Title for your content item."));


            //SchemaBuilder.CreateTable(typeof(HtmlBlockRecord).Name,
            //    table => table
            //        .Column<int>("Id", x => x.PrimaryKey().Identity())
            //        .Column<string>("BlockKey", x => x.WithLength(128).Unique().NotNull())
            //        .Column<string>("HTML", x => x.Unlimited())
            //        .Column<string>("HelpText", x => x.Unlimited())
            //    );
            return 1;
        }

    }
}