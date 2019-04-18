/*************************************************
  Author: Aska Li     
  CreatedDate:  2014/02/21
  function:  内容事件抽象基类
 
  Modified History:        
    1. Date:  2014/02/21
       Author: Aska Li
       Modification:  内容事件抽象基类
*************************************************/
using MnLab.Enterprise.Approval;
using Orchard.ContentManagement;
using Orchard.Security;
using Rhythm;

namespace MnLab.Enterprise.Approval {
    /// <summary>
    /// 内容事件接口
    /// </summary>
    public class ContentEvent /*<TContentPart>*/ : Event
    // where TContentPart : class, IContentPart/*<TContentPart>*///, new()
    {
        /// <summary>
        /// 用户对象
        /// </summary>
        public IUser OperationUser { get; set; }
        /// <summary>
        /// 内容 Id
        /// </summary>
        public string ContentId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public ContentItem Content { get; set; }
        /// <summary>
        /// 是否立即提交审批
        /// </summary>
        public bool IsUserImmediatelyCommit { get; set; }
        /// <summary>
        /// 审批开关
        /// </summary>
        public ApprovalSwitch? Switch { get; set; }
        /// <summary>
        /// 审批对象
        /// </summary>
        public ApprovalPart Approval { get; set; }
        /// <summary>
        /// 获取此事件的详细信息（描述此事件所发生事情）
        /// </summary>
        public override string Message { get { return ""; } }
    }

    /// <summary>
    /// 定义内容“新增”事件
    /// </summary>
    /// <typeparam name="TContent">内容的类型</typeparam>
    public class ContentCreateEvent ///*<TContentPart>*/ : ContentEvent/*<TContentPart>*/
      //  where TContentPart : class, IContentPart/*<TContentPart>*/, new()
    {
        public ContentItem Value { get; set; }
    }

    /// <summary>
    /// 定义内容“修改”事件
    /// </summary>
    /// <typeparam name="TContentPart">内容的类型</typeparam>
    public class ContentEditEvent ///*<TContentPart>*/ : ContentEvent/*<TContentPart>*/
      //  where TContentPart : class, IContentPart/*<TContentPart>*/, new()
    {
        public ContentItem Value { get; set; }
        public int VersionId { get; set; }
    }

    /// <summary>
    /// 定义内容“删除”事件
    /// </summary>
    /// <typeparam name="TContent">内容的类型</typeparam>
    public class ContentDeleteEvent ///*<TContentPart>*/ : ContentEvent/*<TContentPart>*/
     //   where TContentPart : class, IContentPart/*<TContentPart>*/, new()
    {
    }

    /// <summary>
    /// 定义内容“审批”事件
    /// </summary>
    /// <typeparam name="TContent">内容的类型</typeparam>
    public class ContentCommitApprovalEvent/*<TContentPart>*/ : ContentEvent/*<TContentPart>*/
                                                                            // where TContentPart : class, IContentPart/*<TContentPart>*/, new()
    {
    }

}
