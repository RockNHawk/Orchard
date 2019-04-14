namespace Bitlab.Enterprise
{
    public interface IApprovalApproveEvent
    {
        IApproval Approval { get; set; }
 ApprovalSwitch ApprovalSwitch { get; set; }
        global::Rhythm.User ApprovalUser { get; set; }
        global::Rhythm.User CommitUser { get; set; }
        string Message { get; }
    }
}
