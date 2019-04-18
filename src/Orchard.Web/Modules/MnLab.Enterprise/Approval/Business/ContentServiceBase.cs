using Rhythm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Collections.Generic;


using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core;
using Orchard.Data;
using Orchard.Core.Common.Models;
using Orchard.Security;
using Orchard.Localization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Contents.ViewModels;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;
using Orchard.Utility.Extensions;
using Orchard.Localization.Services;
using Orchard.Core.Contents;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval;

using Rhythm;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;

namespace Rhythm {

}

namespace MnLab.Enterprise.Approval {


    //public class ApprovalPartContentRepository : ContentRepository<MnLab.Approval.Models.ApprovalPart> {
    //    public ApprovalPartContentRepository(IContentManager contentManager) : base(contentManager) { }
    //}

    //public class ApprovalStepPartContentRepository : ContentRepository<ApprovalStepRecord> {
    //    public ApprovalStepPartContentRepository(IContentManager contentManager) : base(contentManager) { }
    //}


    /// <summary>
    /// 内容 Service 基类
    /// <para>提供内容新增、编辑、删除、提交审批等服务</para>
    ///  这里原来是一级审批，审批通过直接结束审批,
    /// 现在改为多级，因此增加了：
    ///      一个组织架构（部门）表（bx_Bitlab_Enterprise_Department）
    ///      一个审批明细表（bx_CMS_ApprovalComment）
    ///      每级部门进行一次审批，都会产生审批明细记录，然后审批跳转到上级（Approval.CurrentArchitectureId），当跳转到顶级部门后（字段为 EndCommentOrganizationStructureId），完成审批。
    /// </summary>
    /// <typeparam name="TContentPart">内容的类型（如 Article 文章、File 文件）</typeparam>
    /// <typeparam name="TService">Service 的类型，子类继承时传子类的类型即可</typeparam>

    // [Depedency]
    public class ContentApprovalService : ServiceBase, Orchard.IDependency
//, IContentVersioningService, IApprovalService
// where TContentPart : class, IContentPart, new()
//where TService : class
{
        //protected static System.Type contentType = TypeTable.Type;
        //protected static string contentTypeName = contentType.Name;

        //protected readonly IRepository<ContentRecord> contentRecordRepository = (IRepository<ContentRecord>)RepositoryManager.Default.Of<ContentRecord>();
        //protected readonly NHibernateRepository<ContentPreview> contentPreviewRepository = (NHibernateRepository<ContentPreview>)RepositoryManager.Default.Of<ContentPreview>();
        //protected readonly NHibernateRepository<ContentVersion> contentVersionRepository = (NHibernateRepository<ContentVersion>)RepositoryManager.Default.Of<ContentVersion>();
        //protected readonly NHibernateRepository<IApproval> approvalRepository = (NHibernateRepository<IApproval>)Repository.Current.Of<IApproval>();

        ContentApprovalService approvalService;
        //static int idSequence = 1000;


        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;
        private readonly ICultureFilter _cultureFilter;


        //IRepository<ApprovalSupportPartRecord> _approvalSupportRepos;

        //readonly ContentRepository<ApprovalPart> approvalRepository;
        //readonly ContentRepository<ApprovalStepPart> ApprovalStepRepository;

        public ContentApprovalService(
         //  ContentRepository<ApprovalPart> approvalRepository,
         //  ContentRepository<ApprovalStepRecord> ApprovalStepRepository,
         IRepository<ApprovalStepRecord> ApprovalStepRepository,
        IOrchardServices orchardServices,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            ICultureManager cultureManager,
              IRepository<ApprovalSupportPartRecord> approvalSupportRepos,
        ICultureFilter cultureFilter) {


            //this._approvalSupportRepos = approvalSupportRepos;
            // this.approvalRepository = approvalRepository;
            this.approvalRepository = new ContentRepository<ApprovalPart>(contentManager);
            this.ApprovalStepRepository = ApprovalStepRepository;

            Services = orchardServices;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            _cultureManager = cultureManager;
            _cultureFilter = cultureFilter;



            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;



            approvalService = this;
            //IoC.Default.Register<IContentVersioningService>(this);
            //IoC.Default.Register<ContentVersioningServiceBase>(this);
            //IoC.Default.Register<IApprovalService>(this);

        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public ContentItem GetContent(int contentId) {
            return _contentManager.Get(contentId);
        }

        //public ContentPreview GetContentPreivew(string contentId) {
        //    return contentPreviewRepository.Get(contentId);
        //}


        //public string NewContentId() {
        //    var seq = System.Threading.Interlocked.Increment(ref idSequence);
        //    if (seq > 8000) {
        //        seq = 1000;
        //        idSequence = 1000;
        //    }
        //    return DateTime.Now.ToString("yyyyMMddHHmmss") + seq.ToStringInvariant();// +"_" + contentTypeName;
        //}

        //public virtual TContentPart NewContentPart() {
        //    return new TContentPart { };
        //}

        //public ContentItem Create(Orchard.WorkContext wc, ContentItem content) {
        //    if (content == null) {
        //        throw new ArgumentNullException(nameof(content));
        //    }
        //    var CreatedUtc = content.As<ICommonPart>().CreatedUtc;
        //    if (CreatedUtc.IsEmpty()) {
        //        content.As<ICommonPart>().CreatedUtc = DateTime.Now;
        //    }
        //    //var value = content.Value;
        //    //if (value == null) {
        //    //    throw new ArgumentNullException("content.Value");
        //    //}
        //    if (content.GetDraftVersion(_contentManager) == null) {
        //        var draftValue = new TContentPart();
        //        draftValue.SetValue(value);
        //        content.GetDraftVersion(_contentManager) = new ContentVersion { Value = draftValue, };
        //    }
        //    if (content.ContentId == null) {
        //        content.ContentId = NewContentId();
        //    }
        //    // 用户 xxx 修改了 User#aska
        //    new EntityModificationEvent<User>(new User());

        //    var @event = new ContentCreateEvent {
        //        ContentId = content.ContentId,
        //        Content = content,
        //    };
        //    using (var trace = wc.BeginTrace(@event)) {
        //        contentVersionRepository.Save(content.GetDraftVersion(_contentManager));
        //        contentRecordRepository.Save(content);
        //        trace.Success();
        //        return content;
        //    }
        //}

        //public ContentItem Edit(Orchard.WorkContext wc, ContentItem @event)
        //{
        //}

        ///// <summary>
        ///// 创建内容
        ///// </summary>
        ///// <param name="version">版本信息</param>
        ///// <param name="commitApproval">是否立即提交审批</param>
        ///// <returns></returns>
        //[Rhythm.Transaction.Transactional]
        //public virtual ContentItem Create(Orchard.WorkContext wc, ContentCreateEvent @event) {
        //    if (@event == null) {
        //        throw new ArgumentNullException(nameof(@event));
        //    }
        //    InitEvent(@event);
        //    var value = @event.Value;
        //    if (value == null) {
        //        throw new ArgumentNullException("event.Value");
        //    }
        //    var currentUser = wc.User();
        //    NUnit.Framework.Assert.IsNotNull(currentUser);
        //    if (value.DateCreated.IsEmpty()) {
        //        value.DateCreated = DateTime.Now;
        //    }
        //    value.CreatedBy = currentUser;
        //    if (value.DateCreated.IsEmpty()) {
        //        value.DateCreated = value.DateCreated;
        //    }
        //    var draftVersion = new ContentVersion { Value = value, };
        //    var content = new ContentRecord {
        //        Value = value,
        //        DraftVersion = draftVersion,
        //    };
        //    if (string.IsNullOrEmpty(@event.ContentId)) {
        //        @event.ContentId = NewContentId();
        //    }
        //    content.ContentId = @event.ContentId;
        //    @event.Content = content;
        //    using (var trace = wc.BeginTrace(@event)) {
        //        contentVersionRepository.Save(draftVersion);
        //        contentRecordRepository.Save(content);

        //        @event.Approval = CreateApproval(wc, @event, ApprovalType.Creation);

        //        trace.Success();
        //        return content;
        //    }
        //}

        ///// <summary>
        ///// 编辑内容
        ///// </summary>
        //[Rhythm.Transaction.Transactional]
        //public virtual ContentItem Edit(Orchard.WorkContext wc, ContentEditEvent @event) {
        //    if (@event == null) {
        //        throw new ArgumentNullException(nameof(@event));
        //    }
        //    InitEvent(@event);
        //    var value = @event.Value;
        //    if (value == null) {
        //        throw new ArgumentNullException("event.Version");
        //    }
        //    var content = @event.Content ?? contentRecordRepository.Get(@event.ContentId);
        //    if (content == null) {
        //        throw ValidationEntityNotExistsError(content.ContentId);
        //    }
        //    @event.ContentId = content.ContentId;
        //    @event.Content = content;

        //    SetDefaultValue(value, content);
        //    value.ModifiedBy = wc.User();
        //    var versionId = @event.VersionId;

        //    using (var trace = wc.BeginTrace(@event)) {
        //        string errorMessage = CanEdit(wc, content);
        //        if (errorMessage != null && errorMessage.Length != 0) {
        //            throw ValidationError(errorMessage);
        //        }

        //        var draftVersion = content.GetDraftVersion(_contentManager);
        //        if (draftVersion != null && draftVersion.VersionId == versionId) {
        //            draftVersion.Value = value;
        //            contentVersionRepository.Update(draftVersion);
        //            //contentVersionRepository.Flush();
        //        }
        //        else {
        //            draftVersion = new ContentVersion { Value = value, };
        //            content.GetDraftVersion(_contentManager) = draftVersion;
        //            contentVersionRepository.SaveOrUpdate(draftVersion);
        //        }
        //        @event.Approval = CreateApproval(wc, @event, content.GetCurrentVersion() == null ? ApprovalType.Creation : ApprovalType.Modification);
        //        trace.Success();
        //        return content;
        //    }
        //}

        //private static void SetDefaultValue(TContentPart value, ContentItem content) {
        //    if (value.DateCreated.IsEmpty()) {
        //        value.DateCreated = content.Value.DateCreated;
        //    }
        //    if (value.DateModified.IsEmpty()) {
        //        value.DateModified = DateTime.Now;
        //    }
        //    if (value.CreatedBy == null) {
        //        value.CreatedBy = content.As<CommonPart>().Owner;
        //    }
        //    //if (value.DisplayMode == null)
        //    //{
        //    //    value.DisplayMode = content.Value.DisplayMode;
        //    //}
        //    //if (value.HierarchyId.IsNull)
        //    //{
        //    //    value.HierarchyId = content.Value.HierarchyId;
        //    //}
        //}

        ///// <summary>
        ///// 撤销草稿
        ///// </summary>
        ///// <param name="contentId"></param>
        ///// <returns></returns>
        //public virtual void UndoDraft(Orchard.WorkContext wc, string contentId) {
        //    var content = contentRecordRepository.Get(contentId);
        //    if (content == null) {
        //        throw ValidationEntityNotExistsError(contentId);
        //    }
        //    if (content.GetDraftVersion(_contentManager) == null) {
        //        return;
        //    }
        //    content.GetDraftVersion(_contentManager) = null;
        //    contentRecordRepository.Update(content);
        //}

        ///// <summary>
        ///// 删除，并提交审批
        ///// </summary>
        //[Rhythm.Transaction.Transactional]
        //public virtual ContentItem Delete(Orchard.WorkContext wc, ContentDeleteEvent @event) {
        //    var content = contentRecordRepository.Get(@event.ContentId);
        //    if (content == null) {
        //        throw ValidationEntityNotExistsError(@event.ContentId);
        //    }
        //    @event.Content = content;
        //    InitEvent(@event);

        //    using (var trace = wc.BeginTrace(@event)) {
        //        // 让 lazy load 的字段立即加载。否则，后面 content.Xxx = value 会无效，这不合理呀？
        //        contentRecordRepository.Update(content);
        //        content.Value.ModifiedBy = wc.User();
        //        // 如果内容还是处于新增，草稿状态（即还没有被提交并审批过）
        //        if (content.GetCurrentVersion() == null) {
        //            //因为被驳回的审批也会进入此条件,所以先删除审批内容,再删除协议,否则会引发外间约束
        //            //approvalService.DeleteApprovalRecordByCustomerAggreementId(content.ContentId.ToString());
        //            ////那么不需要审批了，直接删除就可以了
        //            contentRecordRepository.Delete(content);
        //            Logger.Info(StringUtility.Format("用户提交内容#{0}删除，因为从未提交过审批，因此直接执行物理删除操作。", @event.ContentId));
        //        }
        //        else  // 如被审批通过过
        //        {
        //            @event.IsUserImmediatelyCommit = true;
        //            @event.Approval = CreateApproval(wc, @event, ApprovalType.Deletion);
        //            approvalService.Approve(wc, @event.Approval.ApprovalId);
        //        }
        //    }
        //    return content;
        //}

        //static void InitEvent(ContentEvent e) {
        //    if (e.Switch == null) {
        //        e.Switch = GetApprovalSwitch();
        //    }
        //}

        /// <summary>
        /// 批量删除，并提交审批
        /// </summary>
        /// <param name="contentIds">内容 Id集合</param>
        public virtual void BatchDelete(Orchard.WorkContext wc, IEnumerable<string> contentIds) {
            if (contentIds == null || contentIds.Count() == 0) {
                return;
            }
            foreach (var contentId in contentIds.Distinct()) {
                // Delete(wc, new ContentDeleteEvent { ContentId = contentId });
            }
        }

        /// <summary>
        /// 批量提交审批
        /// </summary>
        /// <param name="contentIds">审批 Id 集合</param>
        public virtual void BatchCommitApproval(Orchard.WorkContext wc, int[] contentIds) {
            if (contentIds == null || contentIds.Length == 0) {
                return;
            }
            foreach (var contentId in contentIds.Distinct()) {
                CommmitApproval(wc, contentId, null, false);
            }
        }

        /// <summary>
        /// 提交审批
        /// </summary>
        /// <param name="contentId">内容 Id</param>
        /// <param name="actionType">内容动作</param>
        /// <param name="isThrownIfValidateFail"></param>
        /// <returns>审批对象</returns>
        public ApprovalPart CommmitApproval(Orchard.WorkContext wc, int contentId, System.Type actionType = null, bool isThrownIfValidateFail = true) {
            var content = _contentManager.Get(contentId);
            if (content == null) {
                throw ValidationEntityNotExistsError<ContentItem>(contentId);
            }
            //// 让 lazy load 的字段立即加载。否则，后面 content.Content = value 会无效，这不合理呀？
            //contentRecordRepository.Update(content);
            actionType = actionType ?? (content.GetCurrentVersion() == null ? ApprovalType.Creation : ApprovalType.Modification);
            //if (content.Current == null && content.Draft == null)
            //{
            //    if (!isThrownIfValidateFail)
            //    {
            //        return null;
            //    }
            //    throw ValidationError(string.Format("内容 #{0} 未被修改，不能进行提交审批操作。"));
            //}
            if (ApprovalType.Creation.IsAssignableFrom(actionType)) {
                if (content.GetDraftVersion(_contentManager) == null) {
                    if (!isThrownIfValidateFail) {
                        return null;
                    }
                    throw ValidationError(string.Format("内容 #{0} 没有草稿数据，不能进行新增提交审批操作。", content.Id));
                }
            }
            else if (ApprovalType.Modification.IsAssignableFrom(actionType)) {
                if (content.GetDraftVersion(_contentManager) == null) {
                    if (!isThrownIfValidateFail) {
                        return null;
                    }
                    throw ValidationError(string.Format("内容 #{0} 未被修改，不能进行编辑提交审批操作。", content.Id));
                }
            }
            return approvalService.CommmitApproval(wc, content, actionType);
        }

        //public virtual ContentItem DirectEdit(Orchard.WorkContext wc, ContentEditEvent @event) {
        //    if (@event == null) {
        //        throw new ArgumentNullException(nameof(@event));
        //    }
        //    if (@event.ContentId == null) {
        //        throw new ArgumentNullException("event.ContentId");
        //    }
        //    InitEvent(@event);
        //    var value = @event.Value;
        //    if (value == null) {
        //        throw new ArgumentNullException("event.Version");
        //    }
        //    var versionId = @event.VersionId;
        //    var content = contentRecordRepository.Get(@event.ContentId);
        //    if (content == null) {
        //        throw ValidationEntityNotExistsError(content.ContentId);
        //    }
        //    using (var trace = wc.BeginTrace(@event)) {
        //        SetDefaultValue(value, content);

        //        var version = content.GetDraftVersion(_contentManager) ?? content.GetCurrentVersion();
        //        version.Value = value;
        //        contentVersionRepository.Update(version);

        //        @event.ContentId = content.ContentId;
        //        @event.Content = content;
        //        trace.Success();
        //        return content;
        //    }
        //}


        protected ApprovalPart CreateApproval(Orchard.WorkContext wc, ContentEvent @event, System.Type actionType) {
            var content = @event.Content;
            var approvalSwitch = (@event.Switch ?? GetApprovalSwitch());
            var approval = (@event.IsUserImmediatelyCommit || approvalService.ShoultAotoCommit(@event.OperationUser, approvalSwitch))
                ? approvalService.CommmitApproval(wc, content, actionType) : null;
            return approval;
        }

        public virtual string CanEdit(Orchard.WorkContext wc, IContent content) {
            var contentApproval = content.As<ApprovalSupportPart>();
            if (contentApproval.Status == ApprovalStatus.WaitingApproval) {
                return "此内容正在等待审批（" + contentApproval.GetApprovalTypeDisplayName() + "），因此您不能编辑";
            }
            //if (content.IsDeleted) {
            //    return ("此内容已经删除，您不能编辑。");
            //}
            return null;
        }

        #region IApprovalService


        readonly ContentRepository<ApprovalPart> approvalRepository;
        readonly IRepository<ApprovalStepRecord> ApprovalStepRepository;
        //readonly ContentRepository<ApprovalStepRecord> ApprovalStepRepository;
        //readonly ApprovalRepository approvalRepository = ApprovalRepository.Instance;
        //  readonly IRepository<ApprovalStep> ApprovalStepRepository = RepositoryManager.Default.Of<ApprovalStep>();
        #region Commmit Approval 提交审批
        const string autoApprovalDisabledMessage = "导入操作，自动审批通过。";

        /// <summary>
        /// 提交内容审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="actionType">操作类型</param>
        public virtual ApprovalPart CommmitApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }

            var commitUser = content.As<CommonPart>().Owner;
            //var commitUser = content.As<CommonPart>().Owner;
            var approvalSwitch = GetApprovalSwitch();

            if (ApprovalType.Creation.IsAssignableFrom(actionType)) {
                return CommmitCreationApproval(wc, content, actionType, approvalSwitch, commitUser);
            }
            else if (ApprovalType.Modification.IsAssignableFrom(actionType)) {
                return CommmitModificationApproval(wc, content, actionType, approvalSwitch, commitUser);
            }
            else if (ApprovalType.Deletion.IsAssignableFrom(actionType)) {
                return CommmitDeletionApproval(wc, content, actionType, approvalSwitch, commitUser);
            }
            else {
                throw ValidationError(StringUtility.Format("不正确的自定义 {0}#{1}，自定义 {0} 应从审批流自带的中继承。", actionType.GetType().FullName, actionType));
            }
        }

        /// <summary>
        /// 提交内容 新增 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitCreationApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType, ApprovalSwitch approvalSwitch, IUser commitUser) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            SetContentWaitingApproval(wc, content, actionType);
            var contentType = content.ContentType;
            var approval = new ApprovalPart {
                Record = new ApprovalPartRecord(),
                ApprovalType = actionType,
                CommitDate = DateTime.Now,
                CommitBy = commitUser,
                ContentType = contentType,
            };
            approval.SetContent(content);
            approval.SetContentOldVersion(null);

            var draft = content.GetDraftVersion(_contentManager);
            approval.SetContentNewVersion(draft);
            CreateApproval(wc, approval, approvalSwitch, commitUser);

            //approvalRepository.Flush();

            NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
            return approval;
        }

        /// <summary>
        /// 提交内容 变更 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitModificationApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType, ApprovalSwitch approvalSwitch, IUser commitUser) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.GetDraftVersion(_contentManager) == null) {
                throw new ArgumentNullException("content.GetDraftVersion(_contentManager)");
            }
            SetContentWaitingApproval(wc, content, actionType);
            var contentType = content.ContentType;
            var approval = new ApprovalPart {
                Record = new ApprovalPartRecord(),
                ApprovalType = actionType,
                CommitDate = DateTime.Now,
                CommitBy = wc.CurrentUser,
                ContentType = contentType,
            };
            approval.SetContent(content);

            approval.SetContentOldVersion(content.GetCurrentVersion());
            approval.SetContentNewVersion(content.GetDraftVersion(_contentManager));
            CreateApproval(wc, approval, approvalSwitch, commitUser);
            NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
            return approval;
        }

        /// <summary>
        /// 提交内容 删除 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitDeletionApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType, ApprovalSwitch approvalSwitch, IUser commitUser) {
            SetContentWaitingApproval(wc, content, actionType);
            var approval = new ApprovalPart {
                Record = new ApprovalPartRecord(),
                ApprovalType = actionType,
                CommitDate = DateTime.Now,
                CommitBy = wc.CurrentUser,
                ContentType = content.ContentType,
            };
            approval.SetContent(content);

            approval.SetContentOldVersion(content.GetCurrentVersion());
            approval.SetContentNewVersion(null);
            CreateApproval(wc, approval, approvalSwitch, commitUser);
            NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
            return approval;
        }

        #endregion

        /// <summary>
        /// 审批驳回
        /// </summary>
        /// <param name="approvalId">审批Id</param>
        public virtual ApprovalPart Reject(Orchard.WorkContext wc, int approvalId) {
            return Reject(wc, approvalId, null);
        }

        /// <summary>
        /// 审批驳回
        /// </summary>
        /// <param name="approvalId">审批Id</param>
        /// <param name="comments">审批意见</param>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Reject(Orchard.WorkContext wc, int approvalId, string comments) {
            var approval = (ApprovalPart)approvalRepository.Get(approvalId);
            if (approval == null) {
                throw new ArgumentException(StringUtility.Format("给定的审批#{0}不存在", approvalId), nameof(approvalId));
            }
            return Reject(wc, approval, GetApprovalSwitch(), approval.CommitBy, wc.CurrentUser, comments);
        }

        /// <summary>
        /// 审批驳回
        /// </summary>
        /// <param name="approval">审批对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交用户</param>
        /// <param name="approvalUser">审批用户</param>
        /// <param name="comments">备注</param>
        /// <returns>审批对象</returns>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Reject(Orchard.WorkContext wc, ApprovalPart approval, ApprovalSwitch approvalSwitch, IUser commitUser, IUser approvalUser, string comments = null) {
            if (approval == null) {
                throw new ArgumentNullException(nameof(approval));
            }
            if (commitUser == null) {
                throw new ArgumentNullException(nameof(commitUser));
            }
            if (approvalUser == null) {
                throw new ArgumentNullException(nameof(approvalUser));
            }

            var @event = new ApprovalRejectEvent {
                Approval = approval,
                ApprovalSwitch = approvalSwitch,
                ApprovalUser = approvalUser,
                CommitUser = commitUser,
                Comments = comments
            };

            using (var trace = wc.BeginTrace(@event)) {
                var steps = approval.Steps;
                var currentStep = approval.CurrentStep;

                // validate
                NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
                if (approval.Status != ApprovalStatus.WaitingApproval) {
                    throw ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus.WaitingApproval, approval.Status);
                }
                ValidateCurrentStep(currentStep);
                ValidateDepartment(@event, trace, approvalUser.Department(), currentStep.Department);


                @event.Step = currentStep;
                currentStep.Status = ApprovalStatus.Rejected;
                ApprovalStepRepository.Update(currentStep);
                approval.CurrentStep = null;

                approval.Status = ApprovalStatus.Rejected;
                approval.AuditOpinion = comments;
                approval.AuditDate = DateTime.Now;

                {
                    var contentReflectedType = approval.ContentType;
                    NUnit.Framework.Assert.IsNotNull(contentReflectedType, StringUtility.Format("the Approval#{0}.ContentType is null.", approval.Id));
                    // var contentRepository = RepositoryManager.Default.Of(approval.GetContentRecordType());
                    //var content = approval.ContentRecord;
                    var ContentRecord = approval.ContentRecord;
                    var content = ContentRecord.GetContentItem(_contentManager);
                    var contentApproval = content.As<ApprovalSupportPart>();
                    NUnit.Framework.Assert.IsNotNull(content);
                    contentApproval.Status = ApprovalStatus.Rejected;
                    contentApproval.ApprovalType = approval.ApprovalType;
                    contentApproval.AuditOpinion = comments;
                    RejectContentChange(content, approval.ApprovalType);

                    // Orchard auto update content Object change to Dataabse?
                    //contentRepository.Update(content);
                    // _contentManager.u
                }

                //approvalRepository.Update(approval);

                // 发布审批驳回事件
                PublishEvent(@event);
            }
            return approval;
        }


        /// <summary>
        /// 撤消对内容的更改
        /// </summary>
        /// <param name="content"></param>
        /// <param name="actionType"></param>
        internal static void RejectContentChange(ContentItem content, System.Type actionType) {
        }

        /// <summary>
        /// 创建审批对象
        /// </summary>
        /// <param name="approval">审批对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交用户</param>
        [Rhythm.Transaction.Transactional]
        protected virtual void CreateApproval(Orchard.WorkContext wc, ApprovalPart approval, ApprovalSwitch approvalSwitch, IUser commitUser) {
            if (approval == null) {
                throw new ArgumentNullException(nameof(approval));
            }
            if (commitUser == null) {
                throw new ArgumentNullException(nameof(commitUser));
            }
            approval.Status = ApprovalStatus.WaitingApproval;

            approvalRepository.Save(approval);

            var ContentRecord = approval.ContentRecord;
            var content = ContentRecord.GetContentItem(_contentManager);
            UpdateCurrentApproval(content, approval);

            OnCreateApproval(approval);
            var steps = approval.Steps;
            NUnit.Framework.Assert.IsNotNull(steps);
            NUnit.Framework.Assert.That(steps.Count > 0);
            steps[0].Status = ApprovalStatus.WaitingApproval;

            // If Not On
            if (!approvalSwitch.IsOn()) {
                Approve(wc, approval, approvalSwitch, commitUser, commitUser, true);
            }
        }

        private void UpdateCurrentApproval(ContentItem content, ApprovalPart approval)
        {
            var contentApproval = content.As<ApprovalSupportPart>();
            contentApproval.CurrentApproval = approval.Record;
            // _approvalSupportRepos.Update(contentApproval.Record);
        }

        private void UpdateCurrentApproval2(ApprovalSupportPart contentApproval, ApprovalPart approval)
        {
            contentApproval.CurrentApproval = approval?.Record;
           // _approvalSupportRepos.Update(contentApproval.Record);
        }

        protected virtual void OnCreateApproval(ApprovalPart approval) {

        }

        /// <summary>
        /// 批量执行审批
        /// </summary>
        ApprovalPart[] Batch(Orchard.WorkContext wc, int[] approvalIds, System.Func<Orchard.WorkContext, int, ApprovalPart> approvalHandler) {
            if (approvalIds == null) {
                throw new ArgumentNullException(nameof(approvalIds));
            }
            if (approvalIds.Length == 0) {
                return new ApprovalPart[0];
            }
            approvalIds = approvalIds.Distinct().ToArray();
            if (approvalIds.Length == 1) {
                return new ApprovalPart[] { approvalHandler.Invoke(wc, approvalIds[0]) };
            }
            var errrs = new Dictionary<int, Exception>();
            var approvals = new ApprovalPart[approvalIds.Length];
            for (int i = 0; i < approvals.Length; i++) {
                int approvalId = approvalIds[i];
                //var approval = repository.Get(approvalId);
                //if (approval == null)
                //{
                //    errrs.Add(approvalId, new ServiceException(StringUtility.Format("给定的审批#{0}不存在", approvalId)));
                //    continue;
                //}
                try {
                    approvals[i] = approvalHandler.Invoke(wc, approvalIds[i]);
                }
                catch (Exception ex) {
                    errrs.Add(approvalId, ex);
                }
            }
            if (errrs != null && errrs.Count > 0) {
                if (errrs.Count == 1) {
                    throw errrs[0];
                }
                else {
                    var approvalMsg = new System.Text.StringBuilder(10 * errrs.Count);
                    foreach (var approvalId in errrs.Keys) {
                        var ex = errrs[approvalId];
                        approvalMsg.Append(StringUtility.Format("审批#{0} 失败:{1}\r\n", approvalId, ex.Message));
                    }
                    approvalMsg.Remove((approvalMsg.Length - "\r\n".Length), "\r\n".Length);
                    if (errrs.Count == approvalIds.Length) {
                        throw new AggregateException(StringUtility.Format("批量审批全部失败:{0}", approvalMsg), errrs.Values);
                    }
                    else {
                        throw new AggregateException(StringUtility.Format("批量审批部分失败:{0}", approvalMsg), errrs.Values);
                    }
                }
            }
            return approvals;
        }

        /// <summary>
        /// 批量审批通过
        /// </summary>
        public ApprovalPart[] BatchApprove(Orchard.WorkContext wc, int[] approvalIds) {
            return Batch(wc, approvalIds, Approve);
        }


        /// <summary>
        /// 批量审批驳回
        /// </summary>
        public ApprovalPart[] BatchReject(Orchard.WorkContext wc, int[] approvalIds) {
            return Batch(wc, approvalIds, Reject);
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="approvalId">审批Id</param>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Approve(Orchard.WorkContext wc, int approvalId) {
            var approval = (ApprovalPart)approvalRepository.Get(approvalId);
            if (approval == null) {
                throw new ArgumentException(StringUtility.Format("给定的审批#{0}不存在", approvalId), nameof(approvalId));
            }
            return Approve(wc, approval, GetApprovalSwitch(), approval.CommitBy, wc.User());
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="approval">审批对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交用户</param>
        /// <param name="approvalUser">审批用户</param>
        /// <param name="isAutoApprove">是否自动审批</param>
        /// <returns>审批对象</returns>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Approve(Orchard.WorkContext wc, ApprovalPart approval, ApprovalSwitch approvalSwitch, IUser commitUser, IUser approvalUser, bool isAutoApprove = false) {
            if (approval == null) {
                throw new ArgumentNullException(nameof(approval));
            }
            if (commitUser == null) {
                throw new ArgumentNullException(nameof(commitUser));
            }
            if (approvalUser == null) {
                throw new ArgumentNullException(nameof(approvalUser));
            }
            var steps = approval.Steps;

            var currentStep = approval.CurrentStep;

            var @event = new ApprovalApprove {
                Approval = approval,
                ApprovalSwitch = approvalSwitch,
                ApprovalUser = approvalUser,
                CommitUser = commitUser,
                Step = currentStep,
                Date = DateTime.Now
            };

            ValidateCurrentStep(currentStep);
            using (var trace = wc.BeginTrace(@event)) {
                ValidateDepartment(@event, trace, approvalUser.Department(), currentStep.Department);
                NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
                if (approval.Status != ApprovalStatus.WaitingApproval) {
                    throw trace.Error(ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus.WaitingApproval, approval.Status));
                }

                currentStep.Status = ApprovalStatus.Approved;

                // 如果到了最后一级审批
                if (currentStep.Seq + 1 == steps.Count) {
                    approval.CurrentStep = null;
                    SetApprove(approval, approvalUser, isAutoApprove ? autoApprovalDisabledMessage : null);
                    approvalRepository.Update(approval);
                    ApprovalStepRepository.Update(currentStep);
                    @event.IsCompleted = true;
                }
                else {
                    ApprovalStepRepository.Update(currentStep);
                    var nextStep = steps[currentStep.Seq + 1];
                    nextStep.Status = ApprovalStatus.WaitingApproval;
                    approval.CurrentStep = nextStep;
                    ApprovalStepRepository.Update(currentStep);
                    ApprovalStepRepository.Update(nextStep);
                }
                // 发布审批通过事件
                trace.Success();
            }
            return approval;
        }

        private static void ValidateCurrentStep(ApprovalStepRecord currentStep) {
            if (currentStep == null) {
                throw new Exception("approval.CurrentStep is null");
            }
            if (currentStep.Status != ApprovalStatus.WaitingApproval) {
                throw new Exception("currentStep.Status != ApprovalStatus.WaitingApproval");
            }
        }

        static void ValidateDepartment(Event @event, IEventTrace trace, DepartmentRecord userDept, DepartmentRecord stepDept) {
            if (userDept == null) {
                throw trace.Error(new Exception("您没有所属部门，没有权限进行审批操作"));
            }
            else if (userDept.Id != stepDept.Id) {
                throw trace.Error(new Exception(StringUtility.Format("您所属部门为 {0}，当前审批进程应由 {1} 进行审批。", userDept.Name, stepDept.Name)));
            }
        }

        /// <summary>
        /// 设置审批对象为上级审批通过
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="comments"></param>
        void SetApprove(ApprovalPart approval, IUser approvalByUser, string comments = null) {

            ValidateApproval(approval);
            Debug.Assert(approval.ContentType != null);
            //var contentReflectedType = (approval.ContentType);//.GetReflectedType();
            //if (contentReflectedType == null)
            //{
            //    throw new InvalidOperationException(StringUtility.Format("the ContentType#{0} can't resloved to Reflected Type.", approval.ContentType));
            //}

            // ================================ 审批通过直接结束审批

            //var contentRepository = RepositoryManager.Default.Of(approval.GetContentRecordType());
            //var contentId = approval.GetContentId();
            var ContentRecord = approval.ContentRecord;
            var content = ContentRecord.GetContentItem(_contentManager);
            var contentNewVersion = approval.NewContentVersion;
            //var contentOldVersion = approval.GetContentOldVersion();
            NUnit.Framework.Assert.IsNotNull(content, "Assert:approval.GetContent()!=null");
            //IContent content = (IContent)contentRepository.Get(contentId);
            NUnit.Framework.Assert.IsNotNull(content != null);

            var contentApproval = content.As<ApprovalSupportPart>();

            var approvalType = approval.ApprovalType;
            if (ApprovalType.Creation.IsAssignableFrom(approvalType) || ApprovalType.Modification.IsAssignableFrom(approvalType)) {
                NUnit.Framework.Assert.IsNotNull(contentNewVersion, "Assert:approval.GetContentNewVersion()!=null");
                //content.Value = contentNewVersion.Value;
                // !!Check
                _contentManager.Publish(content);
                //content.GetCurrentVersion() = contentNewVersion;
                //content.GetDraftVersion(_contentManager) = null;
            }
            else if (ApprovalType.Deletion.IsAssignableFrom(approvalType)) {
                //content.Draft = null;
            }
            else if (ApprovalType.Deletion.IsAssignableFrom(approvalType)) {
                // !!Check
                // content.IsDeleted = true;
            }
            else {
                throw new InvalidOperationException("unsupported ApprovalType#" + approval.ApprovalType);
            }

            //contentApproval.CurrentApproval = null;
            UpdateCurrentApproval2(contentApproval, null);
            contentApproval.Status = ApprovalStatus.Approved;
            contentApproval.ApprovalType = approval.ApprovalType;

            // !!Check
            //contentRepository.Update(content);

            approval.AuditOpinion = comments;
            approval.AuditDate = DateTime.Now;
            approval.AuditBy = approvalByUser;
            approval.Status = ApprovalStatus.Approved;
        }

        /// <summary>
        /// 验证上级审批状态
        /// </summary>
        /// <param name="approval"></param>
        static void ValidateApproval(ApprovalPart approval) {
            if (approval.Status != ApprovalStatus.WaitingApproval) {
                throw ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus.WaitingApproval, approval.Status);
            }
        }

        /// <summary>
        /// 验证被审批的内容的状态是否合法
        /// </summary>
        /// <param name="content"></param>
        /// <param name="actionType"></param>
        void SetContentWaitingApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType) {
            NUnit.Framework.Assert.IsNotNull(content.As<CommonPart>().Owner, "content.CreatedByUser is null");
            NUnit.Framework.Assert.IsNotNull(wc.User(), "Orchard.WorkContext.User is null");


            var contentApproval = content.As<ApprovalSupportPart>();
            if (contentApproval.Record == null) {
                var record = new ApprovalSupportPartRecord() {
                    ContentItemRecord = content.Record,
                };
               // _approvalSupportRepos.Create(record);
                contentApproval.Record = record;
            }

            //if (content.ApprovalStatus == ApprovalStatus.Deleted)
            //if (content.IsDeleted) {
            //    throw ValidationError("该内容已被删除，不能提交审批。");
            //}
            if (contentApproval.Status == ApprovalStatus.WaitingApproval) {
                throw ValidationError("该内容已经提交审批，不能重复提交。");
            }
            contentApproval.Status = ApprovalStatus.WaitingApproval;
            contentApproval.ApprovalType = actionType;

            // !!Check
            //RepositoryManager.Default.Of(content.GetContentRecordType()).Update(content);
        }

        /// <summary>
        /// 抛出定义的审批状态验证异常
        /// </summary>
        /// <param name="excepted"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        static System.Exception ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus excepted, ApprovalStatus actual) {
            return ValidationError(StringUtility.Format("审批状态不正确，为“{0}”，理应为“{1}”。", actual.GetDisplayName(), excepted.GetDisplayName()));
        }


        /// <summary>
        /// 获取系统配置的上级审批开关和当前分行的本级审批开关信息
        /// </summary>
        /// <returns></returns>
        internal static ApprovalSwitch GetApprovalSwitch() {
            return ApprovalSwitch.On;
        }

        /// <summary>
        /// 自动提交
        /// </summary>
        /// <returns></returns>
        public bool ShoultAotoCommit(IUser user) {
            return ShoultAotoCommit(user, GetApprovalSwitch());
        }

        /// <summary>
        /// 自动提交
        /// </summary>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="user">用户对象</param>
        /// <returns>是或者否</returns>
        public bool ShoultAotoCommit(IUser user, ApprovalSwitch approvalSwitch) {
            return false;
        }


        public ApprovalPart GetApproval(int approvalId) {
            return (ApprovalPart)approvalRepository.Get(approvalId);
        }

        #endregion



    }
}
