using Orchard.ContentManagement;
using Rhythm;
using System;

namespace MnLab.Enterprise.Approval.Models {
    //using Rhythm.
    public class ApprovalStepRecord {
        public virtual string Id { get; set; }
        //public virtual int ApprovalId { get; set; }
        public virtual ApprovalPartRecord Approval { get; set; }
        public virtual int Seq { get; set; }
        public virtual DepartmentRecord Department { get; set; }
        public virtual ApprovalStatus Status { get; set; }
        public virtual string AuditOpinion { get; set; }
        public virtual Orchard.Security.IUser CommentBy { get; set; }
        public virtual DateTime? AuditDate { get; set; }
    }
}