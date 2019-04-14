using System;
using System.Collections.Generic;


using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core;
using Orchard.Data;


namespace Rhythm {

}

namespace Bitlab.Enterprise {
    using Orchard.Security;
    using Rhythm;

    public class Approval : IApproval
    //<TContentPart> : IApproval where TContentPart : IContentPart<TContentPart>
    {
        /// <summary>
        /// 此审批的 Id
        /// </summary>
        public virtual int ApprovalId { get; set; }

        /// <summary>
        /// 内容的操作类型：新增、编辑、删除、自定义
        /// </summary>
        public virtual System.Type ApprovalType { get; set; }


        string contentType;
        /// <summary>
        /// 内容的类型，NHibernate 根据此字段的值，去 Map 找对应的 Question 子类型，实现多态
        /// </summary>
        public virtual string ContentType {
            get { return contentType; }
            set {
                if (value != null && value != ContentType) {
                    throw new InvalidOperationException("您不能更改 ContentType，它是由 Approval<`1> 的 class 类型自行决定的");
                }
                contentType = value;
            }
        }

        public virtual IList<ApprovalStep> Steps { get; set; }

        ApprovalStep m_CurrentStep;
        public virtual ApprovalStep CurrentStep {
            get { return m_CurrentStep; }
            set {
                m_CurrentStep = value;
                if (value == null) {
                    CurrentStepDepartment = null;
                }
                else {
                    CurrentStepDepartment = m_CurrentStep.Department;
                }
            }
        }

        public virtual Department CurrentStepDepartment { get; set; }

        /// <summary>
        /// 送审日期
        /// </summary>
        public virtual DateTime CommitDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IUser CommitBy { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// 审批的结果:未审批、审批通过、审批驳回
        /// </summary>
        public virtual ApprovalStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IUser CommentsBy { get; set; }

        /// <summary>
        /// 总行审批员执行审批的时间
        /// </summary>
        public virtual DateTime? CommentsDate { get; set; }

        /// <summary>
        /// 总行审批员的审批意见
        /// </summary>
        public virtual string Comments { get; set; }

        //IContentRecord IApproval.ContentRecord { get { return this.ContentRecord; } set { this.ContentRecord = (ContentRecord<TContentPart>)value; } }
        //IContentVersion IApproval.OldContentVersion { get { return this.OldContentVersion; } set { this.OldContentVersion = (ContentVersion<TContentPart>)value; } }
        //IContentVersion IApproval.NewContentVersion { get { return this.NewContentVersion; } set { this.NewContentVersion = (ContentVersion<TContentPart>)value; } }

        /// <summary>
        /// 被审批的文章（如果是文章类型的审批，即 ContentType 是 IllustratedTopic） 
        /// </summary>
        public virtual ContentItem ContentRecord { get; set; }

        /// <summary>
        /// 修改前的文章。（新增文章为 NULL）
        /// </summary>
        public virtual ContentItemVersionRecord NewContentVersion { get; set; }

        /// <summary>
        /// 修改后提交审批的文章
        /// </summary>
        public virtual ContentItemVersionRecord OldContentVersion { get; set; }

        /// <summary>
        /// 获取对象指定的字符串格式
        /// </summary>
        /// <param name="format">指定的格式</param>
        /// <param name="formatProvider">An System.IFormatProvider that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of value of the current object asspecified by provider.</returns>
        public virtual string ToString(string format, IFormatProvider formatProvider) {
            if (format == null) {
                return base.ToString();
            }

            return base.ToString();

            //switch (format) {
            //    default:
            //        throw new FormatException(String.Format("The {0} format string is not supported.", format));
            //    case Formats.DisplayName:
            //    case Formats.EventTitle: {
            //            var newVersion = this.GetContentNewVersion();
            //            var oldVersion = this.GetContentOldVersion();
            //            var contentType = this.ContentType;
            //            var contentTypeDisplayName = contentType == null ? null : contentType.GetDisplayName() ?? contentType.Name;

            //            var approvalType = this.ApprovalType;
            //            if (Bitlab.Enterprise.ApprovalType.Creation.IsAssignableFrom(approvalType)) {
            //                var newVersionTitle = ((IFormattable)newVersion).ToString(Formats.EventTitle, null);
            //                return String.Format("{0}{1}《{2}》", approvalType.GetDisplayName(), contentTypeDisplayName, newVersionTitle);
            //            }
            //            else if (Bitlab.Enterprise.ApprovalType.Modification.IsAssignableFrom(approvalType)) {
            //                var newVersionTitle = ((IFormattable)newVersion).ToString(Formats.EventTitle, null);
            //                var oldVersionTitle = ((IFormattable)oldVersion).ToString(Formats.EventTitle, null);
            //                if (oldVersionTitle != newVersionTitle) {
            //                    return String.Format("{0}{1}《{2}》->《{3}》", approvalType.GetDisplayName(), contentTypeDisplayName, oldVersionTitle, newVersionTitle);
            //                }
            //                else {
            //                    return String.Format("{0}{1}《{2}》", approvalType.GetDisplayName(), contentTypeDisplayName, newVersionTitle);
            //                }
            //            }
            //            else if (Bitlab.Enterprise.ApprovalType.Deletion.IsAssignableFrom(approvalType)) {
            //                var versionTitle = ((IFormattable)(newVersion ?? oldVersion)).ToString(Formats.EventTitle, null);
            //                return String.Format("{0}{1}《{2}》", approvalType.GetDisplayName(), contentTypeDisplayName, versionTitle);
            //            }
            //            else {
            //                throw new NotSupportedException("ActionType#" + ApprovalType + " are not supported.");
            //            }

            //        }
        }
    }



}

}