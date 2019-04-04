using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Indexing;

namespace Contrib.Faq {
    public class ListMigrations : DataMigrationImpl {

        public int Create() {
			// Creating table FaqPartRecord
			SchemaBuilder.CreateTable("FaqListRecord", table => table
				.ContentPartRecord()
				.Column("Category", DbType.String)
				.Column("Intro", DbType.String, column => column.Unlimited())
			);

			ContentDefinitionManager.AlterTypeDefinition("FaqList", cfg => cfg
				.WithPart("CommonPart")
				.WithPart("RoutePart")
				.WithPart("FaqList")
				.WithPart("LocalizationPart")
				.Creatable()
				.Indexed()
			);			
			
            return 1;
        }
    }
}
