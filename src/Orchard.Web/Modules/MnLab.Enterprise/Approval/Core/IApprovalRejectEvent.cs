using Orchard.Users.Models;

namespace MnLab.Enterprise.Approval {
    public interface IApprovalRejectEvent {
        IApproval Approval { get; set; }
        ApprovalSwitch ApprovalSwitch { get; set; }
        UserPartRecord ApprovalUser { get; set; }
        string Comments { get; set; }
        UserPartRecord CommitUser { get; set; }
        string Message { get; }
    }
}
