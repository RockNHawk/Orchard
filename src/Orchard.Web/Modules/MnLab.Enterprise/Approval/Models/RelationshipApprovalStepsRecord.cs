using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;
using MnLab.Enterprise;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core;
using Orchard.Data;
using Orchard.Security;
using System.Linq;

namespace MnLab.Enterprise.Approval.Models {
    public class RelationshipApprovalStepsRecord {
        public virtual int Id { get; set; }
        public virtual ApprovalPartRecord ApprovalPartRecord { get; set; }
        public virtual ApprovalStepRecord ApprovalStepRecord { get; set; }
    }
}