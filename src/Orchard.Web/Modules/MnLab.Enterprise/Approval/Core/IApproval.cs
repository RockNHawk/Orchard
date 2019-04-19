using System;
using System.Collections.Generic;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Security;

namespace MnLab.Enterprise.Approval {
    public interface IApproval : IApprovalInfo {
        int Id { get; }

        //ApprovalStatus Status { get; set; }
        //Type ApprovalType { get; set; }
        //string AuditOpinion { get; set; }

        IUser AuditBy { get; set; }
        DateTime? AuditDate { get; set; }
        IUser CommitBy { get; set; }
        DateTime CommitDate { get; set; }
        ContentItemRecord ContentRecord { get; set; }
        string ContentType { get; set; }
        ApprovalStepRecord CurrentStep { get; set; }
        DepartmentRecord CurrentStepDepartment { get; set; }
        string Message { get; set; }
        ContentItemVersionRecord NewContentVersion { get; set; }
        ContentItemVersionRecord OldContentVersion { get; set; }
        IList<ApprovalStepRecord> Steps { get; set; }
    }
}