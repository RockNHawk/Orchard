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
    public class Migrations : DataMigrationImpl {

        public int Create() {
			// Creating table FaqPartRecord
			SchemaBuilder.CreateTable("FaqPartRecord", table => table
				.ContentPartRecord()
				.Column("Question", DbType.String, column => column.Unlimited())
				.Column("Answer", DbType.String, column => column.Unlimited())
				.Column("Category", DbType.String)
				.Column("SubCategory", DbType.String)
			);
			
			ContentDefinitionManager.AlterTypeDefinition("Faq", cfg => cfg
				.WithPart("CommonPart")
				.WithPart("RoutePart")
				.WithPart("FaqPart")
				.WithPart("TagsPart")
				.WithPart("LocalizationPart")
				.Creatable()
				.Indexed()
			);			

            return 1;
        }
    }
}
