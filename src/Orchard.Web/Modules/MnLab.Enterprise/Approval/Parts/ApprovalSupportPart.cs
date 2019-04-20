﻿using System;
using System.ComponentModel.DataAnnotations;
using MnLab.Enterprise;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using MnLab.Enterprise.Approval.Models;

namespace MnLab.Enterprise.Approval {
    public class ApprovalSupportPart : ContentPart<ApprovalSupportPartRecord>, IApprovalInfo {

        public string CommitOpinion { get { return Record?.CommitOpinion; } set { Record.CommitOpinion = value; } }
        public string AuditOpinion { get { return Record?.AuditOpinion; } set { Record.AuditOpinion = value; } }
        public ApprovalStatus Status { get { return Record?.Status ?? default(ApprovalStatus); } set { Record.Status = value; } }
        public Type ApprovalType { get { return Record?.ApprovalType; } set { Record.ApprovalType = value; } }
        public ApprovalPartRecord Current { get { return Record?.Current; } set { Record.Current = value; } }

        //public virtual string ApprovalComments { get; set; }
        //public virtual ApprovalStatus ApprovalStatus { get; set; }
        //public virtual Type ApprovalType { get; set; }
        //public virtual IApproval CurrentApproval { get; set; }

    }
}