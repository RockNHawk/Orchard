/*************************************************
  Author: Aska Li     
  CreatedDate:  2014/02/21
  function:  定义审批驳回事件
 
  Modified History:        
    1. Date:
       Author: 
       Modification:  
*************************************************/
using Rhythm;
using Drahcro.Data;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using System;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using MnLab.Enterprise.Approval.Models;
using Orchard.Users.Models;

namespace MnLab.Enterprise.Approval {

    /// <summary>
    /// 定义审批通过事件
    /// </summary>
    [TypeDisplay(Name = "审批通过")]
    public class ApprovalApproveCommand : ApprovalEvent {
        //IApproval IApprovalApproveEvent.Approval {
        //    get {
        //        return this.Approval;
        //    }
        //    set {
        //        this.Approval = (ApprovalPart/*<TContentPart>*/)value;
        //    }
        //}

    }
}


