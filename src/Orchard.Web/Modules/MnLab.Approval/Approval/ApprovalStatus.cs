/*************************************************
  Author: Aska Li     
  CreatedDate:  2014/02/21
  function:  审批的状态枚举
 
  Modified History:        
    1. Date:  2014/02/21
       Author: Aska Li
       Modification:  审批的状态枚举
*************************************************/

namespace Bitlab.Enterprise
{
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// 审批的状态
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// 待送审（新增）
        /// </summary>
        [Display(Name = "待送审（新增）")]
        None = 0,

        /// <summary>
        /// 等待审批
        /// </summary>
        [Display(Name = "待审批")]
        WaitingApproval = 10,

        /// <summary>
        /// 审批驳回
        /// </summary>
        [Display(Name = "审批驳回")]
        Rejected = 20,

        /// <summary>
        /// 审批通过
        /// </summary>
        [Display(Name = "审批通过")]
        Approved = 100,
    }
}