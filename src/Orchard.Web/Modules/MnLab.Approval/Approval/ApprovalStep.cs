using Rhythm;
using System;

namespace Bitlab.Enterprise
{
    //using Rhythm.
    public class ApprovalStep
    {
        public virtual string ApprovalStepId { get; set; }
        public virtual int ApprovalId { get; set; }
        public virtual  int Seq { get; set; }
        
        public virtual Department Department { get; set; }
        public virtual ApprovalStatus Status { get; set; }
        public virtual string Comment { get; set; }
        public virtual Orchard.Security.IUser CommentBy { get; set; }
        public virtual DateTime? CommentDate { get; set; }
    }
}