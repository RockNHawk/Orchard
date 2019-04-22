using System;
using MnLab.PdfVisualDesign.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace MnLab.PdfVisualDesign {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable(nameof(TempalteSupportPartRecord),
                table => table
                    .ContentPartVersionRecord()
                    .Column<string>(nameof(TempalteSupportPartRecord.Title), column => column.WithLength(1024))
                );

            ContentDefinitionManager.AlterPartDefinition(nameof(TempalteSupportPart), builder => builder
                .Attachable()
                .WithDescription("Provides a tempalte support for your content item."));


            SchemaBuilder.CreateTable(nameof(PdfGeneratePartRecord),
              table => table
                  .ContentPartVersionRecord()
                  .Column<string>(nameof(PdfGeneratePartRecord.FileName), column => column.WithLength(1024))
                  .Column<DateTime>(nameof(PdfGeneratePartRecord.CreatedUtc))
              );

            ContentDefinitionManager.AlterPartDefinition(nameof(PdfGeneratePart), builder => builder
                .Attachable()
                .WithDescription("Provides a PDF generate support for your content item."));


            //SchemaBuilder.CreateTable(typeof(HtmlBlockRecord).Name,
            //    table => table
            //        .Column<int>("Id", x => x.PrimaryKey().Identity())
            //        .Column<string>("BlockKey", x => x.WithLength(128).Unique().NotNull())
            //        .Column<string>("HTML", x => x.Unlimited())
            //        .Column<string>("HelpText", x => x.Unlimited())
            //    );
            return 2;
        }


        public int UpdateFrom1() {

            ContentDefinitionManager.AlterPartDefinition(nameof(TempalteSupportPart), builder => builder
              .Attachable()
              .WithDescription("Provides a tempalte support for your content item."));
            return 2;
        }



    }
}