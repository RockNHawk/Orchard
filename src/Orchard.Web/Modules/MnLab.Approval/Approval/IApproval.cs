using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Security;

namespace Bitlab.Enterprise {
    public interface IApproval {
        int ApprovalId { get; set; }
        Type ApprovalType { get; set; }
        string Comments { get; set; }
        IUser CommentsBy { get; set; }
        DateTime? CommentsDate { get; set; }
        IUser CommitBy { get; set; }
        DateTime CommitDate { get; set; }
        ContentItem ContentRecord { get; set; }
        string ContentType { get; set; }
        ApprovalStep CurrentStep { get; set; }
        Department CurrentStepDepartment { get; set; }
        string Message { get; set; }
        ContentItemVersionRecord NewContentVersion { get; set; }
        ContentItemVersionRecord OldContentVersion { get; set; }
        ApprovalStatus Status { get; set; }
        IList<ApprovalStep> Steps { get; set; }
    }
}