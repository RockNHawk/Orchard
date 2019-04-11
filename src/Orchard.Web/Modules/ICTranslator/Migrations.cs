using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using ICTranslator.Models;

namespace ICTranslator
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable("ICTranslatorRecord", table => table
                .ContentPartRecord()
                .Column("FromLanguage", DbType.String, column => column.WithLength(20))
                .Column("ToLanguage", DbType.String, column => column.WithLength(20))
                .Column<string>("TranslateText", column => column.Unlimited())
            );


            ContentDefinitionManager.AlterPartDefinition(typeof(ICTranslatorPart).Name,
                 cfg => cfg.Attachable());
            return 1;
        }
    }
}