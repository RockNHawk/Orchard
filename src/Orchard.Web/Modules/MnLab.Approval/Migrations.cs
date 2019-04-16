using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using System;

namespace MnLab.Enterprise.Approval {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            base.SchemaBuilder.CreateTable(nameof(ApprovalSupportPartRecord),
                table =>
                MapApprovalInfo(table)
                .Column<int>(nameof(ApprovalSupportPartRecord.CurrentApproval) + "_Id")
                );

            base.SchemaBuilder.CreateTable(nameof(ApprovalPartRecord),
              table =>
              MapApprovalInfo(table)
               .Column<int>("Id", column => column.PrimaryKey().Identity())
              .Column<int>(nameof(ApprovalPartRecord.CommitBy) + "_Id")
              .Column<int>(nameof(ApprovalPartRecord.AuditBy) + "_Id")
              .Column<int>(nameof(ApprovalPartRecord.ContentRecord) + "_Id")
              .Column<int>(nameof(ApprovalPartRecord.OldContentVersion) + "_Id")
              .Column<int>(nameof(ApprovalPartRecord.NewContentVersion) + "_Id")
              .Column<DateTime>(nameof(ApprovalPartRecord.AuditDate))
              .Column<DateTime>(nameof(ApprovalPartRecord.CommitDate))
              .Column<string>(nameof(ApprovalPartRecord.ContentType))
              .Column<string>(nameof(ApprovalPartRecord.CommitDate))
              );



            ContentDefinitionManager.AlterPartDefinition(nameof(ApprovalSupportPart), builder => builder
                .Attachable()
                .WithDescription("Provides a approval support for your content item."));

            return 2;
        }

        private static Orchard.Data.Migration.Schema.CreateTableCommand MapApprovalInfo(Orchard.Data.Migration.Schema.CreateTableCommand table) {
            return table.ContentPartVersionRecord()
                .Column<string>(nameof(IApprovalInfo.CommitOpinion), column => column.WithLength(1024))
                .Column<string>(nameof(IApprovalInfo.AuditOpinion), column => column.WithLength(1024))
                // TODO: map type to string,map enum to int
                .Column<string>(nameof(IApprovalInfo.ApprovalType), column => column.WithLength(1024))
                .Column<int>(nameof(IApprovalInfo.Status));
        }

        public int UpdateFrom1() {
            //ContentDefinitionManager.AlterPartDefinition("ApprovalSupportPart", builder => builder
            //    .WithDescription("Provides a Title for your content item."));
            return 2;
        }
    }
}