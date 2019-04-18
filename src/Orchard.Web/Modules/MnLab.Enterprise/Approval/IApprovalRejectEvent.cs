namespace MnLab.Enterprise.Approval {
    public interface IApprovalRejectEvent {
        IApproval Approval { get; set; }
        ApprovalSwitch ApprovalSwitch { get; set; }
        Orchard.Security.IUser ApprovalUser { get; set; }
        string Comments { get; set; }
        Orchard.Security.IUser CommitUser { get; set; }
        string Message { get; }
    }
}
