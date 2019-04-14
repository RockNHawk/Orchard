using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace MnLab.Approval.Models {
    public class ApprovalPart : ContentPart<ApprovalPartRecord> {

        //[Required]

        public string UserCommit {
            get { return Retrieve(x => x.UserCommit); }
            set { Store(x => x.UserCommit, value); }
        }


    }
}