using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace MnLab.Approval.Models {
    public class ApprovalPart : ContentPart<ApprovalPartRecord> {

        //[Required]

        public string UserCommit {
            get { return Retrieve(x => x.Title); }
            set { Store(x => x.Title, value); }
        }


    }
}