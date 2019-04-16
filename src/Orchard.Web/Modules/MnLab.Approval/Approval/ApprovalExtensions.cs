/*************************************************
  Author: Aska Li     
  CreatedDate:  2014/02/21
  function: 审批的扩展方法
 
  Modified History:        
    1. Date:  2014/02/21
       Author: Aska Li
       Modification: 审批的扩展方法
*************************************************/
using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Bitlab.Enterprise
{
    /// <summary>
    /// </summary>
    public static class ApprovalExtensions
    {
        static readonly System.Type approvalType = typeof(IApproval);


        public static string GetDisplayName(this ApprovalStatus obj) {
            return obj.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="version"></param>
        public static void SetContentOldVersion(this IApproval approval, ContentItemVersionRecord version)
        {
            approval.OldContentVersion = version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="version"></param>
        public static void SetContentNewVersion(this IApproval approval, ContentItemVersionRecord version)
        {
            approval.NewContentVersion = version;
        }

        //public static ContentItem GetContent(this IApproval approval)
        //{
        //    if (approval.ContentType == null)
        //    {
        //        throw new ArgumentException("approval.ContentType 不应为 null", "approval.ContentType");
        //    }
        //    return approval.ContentRecord;
        //}

        /// <summary>
        /// SetContent
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="content"></param>
        public static void SetContent(this IApproval approval, ContentItem content)
        {
            approval.ContentRecord = content.Record;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="approval"></param>
        ///// <returns></returns>
        //public static string GetContentId(this IApproval approval)
        //{
        //    if (approval.ContentType == null)
        //    {
        //        throw new ArgumentException("approval.ContentType 不应为 null", "approval.ContentType");
        //    }
        //    return approval.ContentRecord.ContentId;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="approval"></param>
        ///// <returns></returns>
        //public static System.Type GetContentRecordType(this IApproval approval)
        //{
        //    if (approval.ContentType == null)
        //    {
        //        throw new ArgumentException("approval.ContentType 不应为 null", "approval.ContentType");
        //    }
        //    return typeof(ContentRecord<>).MakeGenericType(approval.ContentType);
        //}

        ///// <summary>
        ///// SetContentId
        ///// </summary>
        ///// <param name="approval"></param>
        ///// <param name="contentId"></param>
        //public static void SetContentId(this IApproval approval, string contentId)
        //{
        //    approval.ContentRecord.Id = contentId;
        //    //approval.ContentRecord.ContentId = contentId;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        public static ContentItemVersionRecord GetContentOldVersion(this IApproval approval)
        {
            return approval.OldContentVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        public static ContentItemVersionRecord GetContentNewVersion(this IApproval approval)
        {
            return approval.NewContentVersion;
        }
 

        public static ContentItemVersionRecord GetDraftVersion(this ContentItem content,IContentManager _contentManager) {
         var con=    _contentManager.Get(content.Id,VersionOptions.Draft);
            return con.VersionRecord;
        }

        public static ContentItemVersionRecord GetCurrentVersion(this ContentItem content) {
            return content.VersionRecord;
        }


        //static System.Reflection.PropertyInfo GetContentProperty(IApproval approval)
        //{
        //    return GetContentPrefixProperty(approval, null);
        //}

        //static System.Reflection.PropertyInfo GetContentIdProperty(IApproval approval)
        //{
        //    return GetContentPrefixProperty(approval, "Id");
        //}
        //static System.Reflection.PropertyInfo GetContentOldVersionProperty(IApproval approval)
        //{
        //    return GetContentPrefixProperty(approval, "OldVersion");
        //}
        //static System.Reflection.PropertyInfo GetContentNewVersionProperty(IApproval approval)
        //{
        //    return GetContentPrefixProperty(approval, "NewVersion");
        //}
        //static System.Reflection.PropertyInfo GetContentPrefixProperty(IApproval approval, string name)
        //{
        //    string propertyName = approval.ContentType.Name + name;
        //    var contentIdProperty = approvalType.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        //    if (contentIdProperty == null)
        //    {
        //        throw new MissingMemberException(approvalType.FullName, propertyName);
        //    }
        //    return contentIdProperty;
        //}


    }
}