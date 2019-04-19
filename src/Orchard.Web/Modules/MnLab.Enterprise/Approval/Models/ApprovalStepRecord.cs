using Orchard.ContentManagement;
using Orchard.Users.Models;
using Rhythm;
using System;

namespace MnLab.Enterprise.Approval.Models {
    //using Rhythm.
    public class ApprovalStepRecord {
        public virtual int Id { get; set; }
        public virtual ApprovalPartRecord Approval { get; set; }
        public virtual int Seq { get; set; }
        public virtual DepartmentRecord Department { get; set; }
        public virtual ApprovalStatus Status { get; set; }
        public virtual string AuditOpinion { get; set; }
        public virtual UserPartRecord AuditBy { get; set; }
        public virtual DateTime? AuditDate { get; set; }
    }
}