namespace MnLab.Enterprise.Approval {
    public interface IApprovalApproveEvent {
        IApproval Approval { get; set; }
        ApprovalSwitch ApprovalSwitch { get; set; }
        Orchard.Security.IUser ApprovalUser { get; set; }
        Orchard.Security.IUser CommitUser { get; set; }
        string Message { get; }
    }
}
