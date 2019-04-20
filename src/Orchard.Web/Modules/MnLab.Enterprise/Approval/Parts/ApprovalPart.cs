using System.ComponentModel.DataAnnotations;
using MnLab.Enterprise;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using System;
using System.Collections.Generic;


using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core;
using Orchard.Data;
using Orchard.Security;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using Orchard.Users.Models;

namespace MnLab.Enterprise.Approval {

    public class ApprovalPart : ContentPart<ApprovalPartRecord>, IApproval {

        ///// <summary>
        ///// 此审批的 Id
        ///// </summary>
        //public virtual int Id { get { return Record.Id; } set { Record.Id = value; } }

        /// <summary>
        /// 内容的操作类型：新增、编辑、删除、自定义
        /// </summary>
        public virtual System.Type ApprovalType { get { return Record.ApprovalType; } set { Record.ApprovalType = value; } }


       // string contentType;
        /// <summary>
        /// 内容的类型，NHibernate 根据此字段的值，去 Map 找对应的 Question 子类型，实现多态
        /// </summary>
        public virtual string ContentType {
            get { return Record.ContentType; }
            set {
                Record.ContentType = value;
            }
        }

        /// <summary>
        /// 送审日期
        /// </summary>
        public virtual DateTime CommitDate { get { return Record.CommitDate; } set { Record.CommitDate = value; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Message { get { return Record.Message; } set { Record.Message = value; } }

        /// <summary>
        /// 审批的结果:未审批、审批通过、审批驳回
        /// </summary>
        public virtual ApprovalStatus Status { get { return Record.Status; } set { Record.Status = value; } }


        public virtual IList<ApprovalStepRecord> Steps { get { return Record.Steps; } set { Record.Steps = value; } }

        public virtual ApprovalStepRecord CurrentStep { get { return Record.CurrentStep; } set { Record.CurrentStep = value; } }

        public virtual DepartmentRecord CurrentStepDepartment { get { return Record.CurrentStepDepartment; } set { Record.CurrentStepDepartment = value; } }

        /// <summary>
        /// 提交人
        /// </summary>
        public virtual UserPartRecord CommitBy { get { return Record.CommitBy; } set { Record.CommitBy = value; } }
        /// <summary>
        /// 提交审批时填写的消息（送审意见）
        /// </summary>
        public string CommitOpinion { get { return Record.CommitOpinion; } set { Record.CommitOpinion = value; } }


        public virtual UserPartRecord AuditBy { get { return Record.AuditBy; } set { Record.AuditBy = value; } }

        /// <summary>
        /// 总行审批员执行审批的时间
        /// </summary>
        public virtual DateTime? AuditDate { get { return Record.AuditDate; } set { Record.AuditDate = value; } }

        /// <summary>
        /// 总行审批员的审批意见
        /// </summary>
        public virtual string AuditOpinion { get { return Record.AuditOpinion; } set { Record.AuditOpinion = value; } }


        /// <summary>
        /// 被审批的文章（如果是文章类型的审批，即 ContentType 是 IllustratedTopic） 
        /// </summary>
        public virtual ContentItemRecord ContentRecord { get { return Record.ContentRecord; } set { Record.ContentRecord = value; } }

        /// <summary>
        /// 修改前的文章。（新增文章为 NULL）
        /// </summary>
        public virtual ContentItemVersionRecord NewContentVersion { get { return Record.NewContentVersion; } set { Record.NewContentVersion = value; } }

        /// <summary>
        /// 修改后提交审批的文章
        /// </summary>
        public virtual ContentItemVersionRecord OldContentVersion { get { return Record.OldContentVersion; } set { Record.OldContentVersion = value; } }


        //IContentRecord IApproval.ContentRecord { get { return this.ContentRecord; } set { this.ContentRecord = (ContentRecord/*<TContentPart>*/)value; } }
        //IContentVersion IApproval.OldContentVersion { get { return this.OldContentVersion; } set { this.OldContentVersion = (ContentVersion/*<TContentPart>*/)value; } }
        //IContentVersion IApproval.NewContentVersion { get { return this.NewContentVersion; } set { this.NewContentVersion = (ContentVersion/*<TContentPart>*/)value; } }


        /// <summary>
        /// 获取对象指定的字符串格式
        /// </summary>
        /// <param name="format">指定的格式</param>
        /// <param name="formatProvider">An System.IFormatProvider that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of value of the current object asspecified by provider.</returns>
        public virtual string ToString(string format, IFormatProvider formatProvider) {
            return Record?.ToString(format, formatProvider);
        }


    }
}