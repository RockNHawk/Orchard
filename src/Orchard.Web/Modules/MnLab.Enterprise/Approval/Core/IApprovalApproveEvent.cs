using Orchard.Users.Models;
namespace MnLab.Enterprise.Approval {
    public interface IApprovalApproveEvent {
        IApproval Approval { get; set; }
        ApprovalSwitch ApprovalSwitch { get; set; }
        UserPartRecord ApprovalUser { get; set; }
        UserPartRecord CommitUser { get; set; }
        string Message { get; }
    }
}
