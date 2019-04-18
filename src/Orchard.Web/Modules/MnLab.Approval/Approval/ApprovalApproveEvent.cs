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
using MnLab.Enterprise.Approval;
using System;
using MnLab.Enterprise.Approval;

namespace MnLab.Enterprise.Approval {
    /// <summary>
    /// 定义审批通过事件
    /// </summary>
    [TypeDisplay(Name = "审批通过")]
    public class ApprovalApprove/*<TContentPart>*/ : Event, IApprovalApproveEvent
    // where TContentPart : class, IContentPart/*<TContentPart>*/, new()
    {
        /// <summary>
        /// 审批信息
        /// </summary>
        public ApprovalPart/*<TContentPart>*/ Approval { get; set; }

        public ApprovalStepRecord Step { get; set; }

        public bool IsCompleted { get; set; }

        /// <summary>
        /// 审批开关
        /// </summary>
        public ApprovalSwitch ApprovalSwitch { get; set; }
        /// <summary>
        /// 此审批的提交者
        /// </summary>
        public Orchard.Security.IUser CommitUser { get; set; }
        /// <summary>
        /// 此审批的审批者
        /// </summary>
        public Orchard.Security.IUser ApprovalUser { get; set; }


        /// <summary>
        /// 获取此事件的详细信息（描述此事件所发生事情）
        /// </summary>
        public override string Message {
            get {
                return StringUtility.Format("审批#{0} -> 被审批员 {1} 通过 ", Approval.ToString(Formats.EventTitle, null), ApprovalUser.Username());
            }
        }

        public DateTime Date { get; set; }


        IApproval IApprovalApproveEvent.Approval {
            get {
                return this.Approval;
            }
            set {
                this.Approval = (ApprovalPart/*<TContentPart>*/)value;
            }
        }

    }
}
