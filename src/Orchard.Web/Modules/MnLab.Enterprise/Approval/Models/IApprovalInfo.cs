using System;
using MnLab.Enterprise;

namespace MnLab.Enterprise.Approval {
    public interface IApprovalInfo {
        Type ApprovalType { get; set; }
        string AuditOpinion { get; set; }
        string CommitOpinion { get; set; }
        ApprovalStatus Status { get; set; }
    }
}