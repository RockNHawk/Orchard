namespace Bitlab.Enterprise
{
    public interface IApprovalRejectEvent
    {
     IApproval Approval { get; set; }
        ApprovalSwitch ApprovalSwitch { get; set; }
        Rhythm.User ApprovalUser { get; set; }
        string Comments { get; set; }
        Rhythm.User CommitUser { get; set; }
        string Message { get; }
    }
}
