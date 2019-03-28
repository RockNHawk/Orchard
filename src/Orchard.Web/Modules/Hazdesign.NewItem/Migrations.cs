using System.Data;
using Orchard.Data.Migration;
using Orchard.Core.Contents.Extensions;
using Orchard.ContentManagement.MetaData;
namespace Hazdesign.NewItem {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("NewItemPart", cfg =>
                 cfg.Attachable());
            
            return 1;
        }
        public int UpdateFrom1() {
			// Creating table NewItemSettingsPartRecord
			SchemaBuilder.CreateTable("NewItemSettingsPartRecord", table => table
				.ContentPartRecord()
				.Column("Days", DbType.Int32)
			);
            
            return 2;
        }
    }
}