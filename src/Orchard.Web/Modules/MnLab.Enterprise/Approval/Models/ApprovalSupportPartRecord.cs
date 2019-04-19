using System;
using System.ComponentModel.DataAnnotations;
using MnLab.Enterprise;
using Orchard.ContentManagement.Records;
/// <summary>
/*
 *
 * https://articles.runtings.co.uk/2015/10/orchard-cms-quick-tipno-persister-for.html
No Persister for: SomePartRecord
If you’re making a module with a record class and you start seeing errors saying that there is no persister for the part record class you’re working on then you have probably made this simple mistake.

Once recently I started writing a quick module and thought to myself, I don’t need all of these extra namespaces, everything can just go in the general module namespace.

I set about coding and everything seemed to be going well but then I see this error:

No Persister for: SomePartRecord

You might also see this error if you accidentally made your record class in the root folder of the module and then dragged it over to the .\model\ folder.

The problem is that for Autofac to work it requires two criteria for it to be able to find your model:

Ensure that the models (ContentItemPart and ContentItemPartRecord) are in the .\Models\ folder
Ensure that the namespace wrapped around those classes ends in .Models
If you fix these two items then you will also fix the No Persister for: SomePartRecord error.

 */
/// </summary>
namespace MnLab.Enterprise.Approval.Models {
    public class ApprovalSupportPartRecord : ContentPartRecord, IApprovalInfo {
        [StringLength(1024)]
        public virtual string CommitOpinion { get; set; }
        public virtual string AuditOpinion { get; set; }
        public virtual ApprovalStatus Status { get; set; }
        public virtual Type ApprovalType { get; set; }
        public virtual ApprovalPartRecord Current { get; set; }
    }
}
