/*************************************************
  Author: Aska Li     
  CreatedDate:  2014/02/21
  function:  定义审批驳回事件
 
  Modified History:        
    1. Date:
       Author: 
       Modification:  
*************************************************/
using Orchard.Security;
using Rhythm;

namespace Bitlab.Enterprise
{
    /// <summary>
    /// 定义审批驳回事件
    /// </summary>
    [TypeDisplay(Name = "审批驳回")]
    public class ApprovalRejectEvent : Event //, IApprovalRejectEvent
       // where TContentPart : class, IContentPart<TContentPart>, new()
    {
        /// <summary>
        /// 审批信息
        /// </summary>
        public Approval Approval { get; set; }
        /// <summary>
        /// 审批开关
        /// </summary>
        public ApprovalSwitch ApprovalSwitch { get; set; }
        /// <summary>
        /// 此审批的提交者
        /// </summary>
        public IUser CommitUser { get; set; }
        /// <summary>
        /// 此审批的审批者
        /// </summary>
        public IUser ApprovalUser { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 获取此事件的详细信息（描述此事件所发生事情）
        /// </summary>
        public override string Message
        {
            get
            {
                return StringUtility.Format("审批#{0} -> 被审批员 {1} 驳回 ", Approval.ToString(Formats.EventTitle, null), ApprovalUser.Username());
            }
        }

        //IApproval IApprovalRejectEvent.Approval
        //{
        //    get
        //    {
        //        return this.Approval;
        //    }
        //    set
        //    {
        //        this.Approval = (Approval<TContentPart>)value;
        //    }
        //}


        public ApprovalStep Step { get; set; }
    }
}
