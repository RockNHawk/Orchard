using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using System;
using MnLab.Enterprise.Approval.Models;

namespace MnLab.Enterprise.Approval {
    public class Migrations : DataMigrationImpl {

        public int Create() {


            SchemaBuilder.CreateTable(nameof(DepartmentRecord),
                table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>(nameof(DepartmentRecord.Name))
                );


            base.SchemaBuilder.CreateTable(nameof(ApprovalPartRecord),
              table =>
              MapApprovalInfo(table)
              .ContentPartRecord()
              // .Column<int>("Id", column => column.PrimaryKey().Identity())
              .Column<int>(nameof(ApprovalPartRecord.CommitBy) + "_id")
              .Column<int>(nameof(ApprovalPartRecord.AuditBy) + "_id")
              .Column<int>(nameof(ApprovalPartRecord.ContentRecord) + "_id")
              .Column<int>(nameof(ApprovalPartRecord.OldContentVersion) + "_id")
              .Column<int>(nameof(ApprovalPartRecord.NewContentVersion) + "_id")
              .Column<string>(nameof(ApprovalPartRecord.ContentType))
              .Column<DateTime>(nameof(ApprovalPartRecord.AuditDate))
              .Column<DateTime>(nameof(ApprovalPartRecord.CommitDate))
              );


            /// <summary>
            /// Creating-1-n-and-n-n-relations
            /// http://docs.orchardproject.net/en/latest/Documentation/Creating-1-n-and-n-n-relations/
            /// </summary>
            SchemaBuilder.CreateTable(nameof(ApprovalStepRecord),
              table => table
                  .Column<int>("Id", column => column.PrimaryKey().Identity())
                  .Column<string>(nameof(ApprovalStepRecord.AuditOpinion))
                  .Column<DateTime>(nameof(ApprovalStepRecord.AuditDate))
                  .Column<int>($"{nameof(ApprovalStepRecord.Approval)}_id")
                  .Column<int>($"{nameof(ApprovalStepRecord.CommentBy)}_id")
                  .Column<int>($"{nameof(ApprovalStepRecord.Department)}_id")
              );

            //SchemaBuilder.CreateTable(nameof(RelationshipApprovalStepsRecord),
            //  table => table
            //      .Column<int>("Id", column => column.PrimaryKey().Identity())
            //      .Column<int>($"{nameof(RelationshipApprovalStepsRecord.ApprovalPartRecord)}_id")
            //      .Column<int>($"{nameof(RelationshipApprovalStepsRecord.ApprovalStepRecord)}_id")
            //  );

            ContentDefinitionManager.AlterPartDefinition(nameof(ApprovalPart), builder => builder
                .Attachable()
                .WithDescription("Provides approval feature."));


            base.SchemaBuilder.CreateTable(nameof(ApprovalSupportPartRecord),
             table =>
             MapApprovalInfo(table)
             .ContentPartRecord()
             .Column<int>(nameof(ApprovalSupportPartRecord.Latest) + "_id")
             );

            ContentDefinitionManager.AlterPartDefinition(nameof(ApprovalSupportPart), builder => builder
                .Attachable()
                .WithDescription("Provides approval support for your content item."));

            return 1;
        }

        private static Orchard.Data.Migration.Schema.CreateTableCommand MapApprovalInfo(Orchard.Data.Migration.Schema.CreateTableCommand table) {
            return
                table.Column<string>(nameof(IApprovalInfo.CommitOpinion), column => column.WithLength(1024))
                .Column<string>(nameof(IApprovalInfo.AuditOpinion), column => column.WithLength(1024))
                // TODO: map type to string,map enum to int
                .Column<string>(nameof(IApprovalInfo.ApprovalType), column => column.WithLength(1024))
                .Column<int>(nameof(IApprovalInfo.Status));
        }

        //public int UpdateFrom1() {

        //    return 3;
        //}


        //public int UpdateFrom2() {



        //    return 3;
        //}

    }
}