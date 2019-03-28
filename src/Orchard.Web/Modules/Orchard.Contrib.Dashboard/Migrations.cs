using Orchard.Data.Migration;

namespace Orchard.Contrib.Dashboard {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("DashboardItemRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<int>("UserId")
                    .Column<string>("Name", c => c.WithLength(64))
                    .Column<string>("Category", c => c.WithLength(64))
                    .Column<string>("Type", c => c.WithLength(64))
                    .Column<string>("Parameters", c => c.Unlimited())
                    .Column<int>("Position")
                    .Column<bool>("Enabled")
                );

            return 1;
        }
    }
}