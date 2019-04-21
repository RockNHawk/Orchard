using Rhythm;
using Drahcro.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Collections.Generic;

using Orchard.Users.Models;

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
using Drahcro.Data;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;

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


        IRepository<ApprovalSupportPartRecord> _approvalSupportRepos;
        IRepository<ApprovalPartRecord> _approvalPartRecordRepos;
        readonly IContentPartRepository<ApprovalPart, ApprovalPartRecord> _approvalRepository;
        readonly IRepository<ApprovalStepRecord> ApprovalStepRepository;

        public ContentApprovalService(
            IContentPartRepository<ApprovalPart, ApprovalPartRecord> approvalRepository,
         //  ContentRepository<ApprovalStepRecord> ApprovalStepRepository,
         IRepository<ApprovalStepRecord> approvalStepRepository,
            IRepository<ApprovalPartRecord> approvalPartRecordRepos,
              IRepository<ApprovalSupportPartRecord> approvalSupportRepos,
        IOrchardServices orchardServices,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            ICultureManager cultureManager,
        ICultureFilter cultureFilter) {

            this._approvalSupportRepos = approvalSupportRepos;
            this._approvalRepository = approvalRepository;
            this._approvalPartRecordRepos = approvalPartRecordRepos;
            this.ApprovalStepRepository = approvalStepRepository;

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

        ///// <summary>
        ///// 批量提交审批
        ///// </summary>
        ///// <param name="contentIds">审批 Id 集合</param>
        //public virtual void BatchCommitApproval(Orchard.WorkContext wc, int[] contentIds) {
        //    if (contentIds == null || contentIds.Length == 0) {
        //        return;
        //    }
        //    foreach (var contentId in contentIds.Distinct()) {
        //        CommmitApproval(wc, contentId, null, false);
        //    }
        //}

        ///// <summary>
        ///// 提交审批
        ///// </summary>
        ///// <param name="contentId">内容 Id</param>
        ///// <param name="actionType">内容动作</param>
        ///// <param name="isThrownIfValidateFail"></param>
        ///// <returns>审批对象</returns>
        //public ApprovalPart CommmitApproval(Orchard.WorkContext wc, int contentId, System.Type actionType = null, bool isThrownIfValidateFail = true) {
        //    var content = _contentManager.Get(contentId);
        //    if (content == null) {
        //        throw ValidationEntityNotExistsError<ContentItem>(contentId);
        //    }
        //    //// 让 lazy load 的字段立即加载。否则，后面 content.Content = value 会无效，这不合理呀？
        //    //contentRecordRepository.Update(content);
        //    actionType = actionType ?? (content.GetCurrentVersion() == null ? ApprovalType.Creation : ApprovalType.Modification);
        //    //if (content.Current == null && content.Draft == null)
        //    //{
        //    //    if (!isThrownIfValidateFail)
        //    //    {
        //    //        return null;
        //    //    }
        //    //    throw ValidationError(string.Format("内容 #{0} 未被修改，不能进行提交审批操作。"));
        //    //}
        //    if (ApprovalType.Creation.IsAssignableFrom(actionType)) {
        //        if (content.GetDraftVersion(_contentManager) == null) {
        //            if (!isThrownIfValidateFail) {
        //                return null;
        //            }
        //            throw ValidationError(string.Format("内容 #{0} 没有草稿数据，不能进行新增提交审批操作。", content.Id));
        //        }
        //    }
        //    else if (ApprovalType.Modification.IsAssignableFrom(actionType)) {
        //        if (content.GetDraftVersion(_contentManager) == null) {
        //            if (!isThrownIfValidateFail) {
        //                return null;
        //            }
        //            throw ValidationError(string.Format("内容 #{0} 未被修改，不能进行编辑提交审批操作。", content.Id));
        //        }
        //    }
        //    return approvalService.CommmitApproval(wc, content, actionType);
        //}

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



        public virtual string CanEdit(Orchard.WorkContext wc, IContent content) {
            var contentApproval = GetContentApprovalSupport(content.ContentItem);
            if (contentApproval.Status == ApprovalStatus.WaitingApproval) {
                return "此内容正在等待审批（" + contentApproval.GetApprovalTypeDisplayName() + "），因此您不能编辑";
            }
            //if (content.IsDeleted) {
            //    return ("此内容已经删除，您不能编辑。");
            //}
            return null;
        }

        #region IApprovalService



        //readonly ContentRepository<ApprovalStepRecord> ApprovalStepRepository;
        //readonly ApprovalRepository approvalRepository = ApprovalRepository.Instance;
        //  readonly IRepository<ApprovalStep> ApprovalStepRepository = RepositoryManager.Default.Of<ApprovalStep>();
        #region Commmit Approval 提交审批
        const string autoApprovalDisabledMessage = "导入操作，自动审批通过。";

        /// <summary>
        /// 提交内容审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalType">操作类型</param>
        public virtual ApprovalPart CommmitApproval(Orchard.WorkContext wc, CreateApprovalCommand command) {
            if (command.ContentItem == null) throw new ArgumentNullException(nameof(command.ContentItem));

            var content = command.ContentItem;
            var approvalType = command.ApprovalType;
            //var commitUser = content.As<CommonPart>().Owner;

            var approvalSwitch = command.Switch ?? (command.Switch = GetApprovalSwitch());

            var contentApproval = GetContentApprovalSupport(content);
            if (contentApproval.Status == ApprovalStatus.WaitingApproval) {
                throw ValidationError("该内容已经提交审批，不能重复提交。");
            }

            if (command.CommitBy == null) {
                command.CommitBy = wc.User();
            }

            ApprovalPart approval;
            if (ApprovalType.Creation.IsAssignableFrom(approvalType)) {
                approval = CommmitCreationApproval(wc, command);
            }
            else if (ApprovalType.Modification.IsAssignableFrom(approvalType)) {
                approval = CommmitModificationApproval(wc, command);
            }
            else if (ApprovalType.Deletion.IsAssignableFrom(approvalType)) {
                approval = CommmitDeletionApproval(wc, command);
            }
            else {
                throw ValidationError(StringUtility.Format("不正确的自定义 {0}#{1}，自定义 {0} 应从审批流自带的中继承。", approvalType.GetType().FullName, approvalType));
            }

            UpdateCurrentApproval(contentApproval, approval);

            //contentApproval.CurrentApproval = approval.Record;

            //SetContentWaitingApproval(wc, content, command.ApprovalType);

            //var approval = (@event.IsUserImmediatelyCommit || approvalService.ShoultAotoCommit(@event.OperationUser, approvalSwitch))
            //    ? approvalService.CommmitApproval(wc, content, actionType) : null;
            //return approval;
            //}

            // If Not On
            //if (!approvalSwitch.IsOn()) {
            //    Approve(wc, approval, approvalSwitch, commitUser, commitUser, true);
            //}

            return approval;
        }

        /// <summary>
        /// 提交内容 新增 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitCreationApproval(Orchard.WorkContext wc, CreateApprovalCommand command) {
            //if (content == null) {
            //    throw new ArgumentNullException(nameof(content));
            //}
            //var contentType = content.ContentType;
            //var approval = new ApprovalPart {
            //    Record = new ApprovalPartRecord(),
            //    ApprovalType = actionType,
            //    CommitDate = DateTime.Now,
            //    CommitBy = commitUser,
            //    ContentType = contentType,
            //};
            //approval.SetReferenceContent(content);
            //approval.SetContentOldVersion(null);

            var content = command.ContentItem;
            var draft = content.GetDraftVersion(_contentManager);
            command.NewContentVersion = draft;
            return CreateApproval(wc, command);
        }


        /// <summary>
        /// 提交内容 变更 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitModificationApproval(Orchard.WorkContext wc, CreateApprovalCommand command) {
            var content = command.ContentItem;
            var draft = content.GetDraftVersion(_contentManager);
            command.OldContentVersion = content.GetCurrentVersion();
            command.NewContentVersion = content.GetDraftVersion(_contentManager);
            return CreateApproval(wc, command);

            //approvalPart.SetReferenceContent(content);
            //    approvalPart.SetContentOldVersion(content.GetCurrentVersion());
            //    approvalPart.SetContentNewVersion(content.GetDraftVersion(_contentManager));
            //    CreateApproval(wc, approvalPart, content, approvalSwitch, commitUser);
            //    NUnit.Framework.Assert.AreNotEqual(0, approvalPart.Id);
            //return approvalPart;
        }

        /// <summary>
        /// 提交内容 删除 审批
        /// </summary>
        /// <param name="content">内容对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交人</param>
        /// <returns>审批对象</returns>
        public virtual ApprovalPart CommmitDeletionApproval(Orchard.WorkContext wc, CreateApprovalCommand command) {
            var content = command.ContentItem;
            var draft = content.GetDraftVersion(_contentManager);
            command.OldContentVersion = content.GetCurrentVersion();
            command.NewContentVersion = null;
            return CreateApproval(wc, command);
            //SetContentWaitingApproval(wc, content, actionType);
            //var approvalPart = CreateApproval(new ApprovalPartRecord {
            //    ApprovalType = actionType,
            //    CommitDate = DateTime.Now,
            //    CommitBy = commitUser,// wc.CurrentUser.As<UserPart>().Record,
            //    ContentType = content.ContentType,
            //    OldContentVersion = content.GetCurrentVersion(),
            //    NewContentVersion = null,
            //}, content);

            ////approvalPart.SetContentOldVersion(content.GetCurrentVersion());
            ////approvalPart.SetContentNewVersion(null);
            ////CreateApproval(wc, approvalPart, content, approvalSwitch, commitUser);
            //NUnit.Framework.Assert.AreNotEqual(0, approvalPart.Id);
            //return approvalPart;
        }





        /// <summary>
        /// 创建审批对象
        /// </summary>
        /// <param name="approval">审批对象</param>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="commitUser">提交用户</param>
        public ApprovalPart CreateApproval(Orchard.WorkContext wc, CreateApprovalCommand command) {
            var content = command.ContentItem;
            var approvalSwitch = (command.Switch ?? GetApprovalSwitch());

            var commitUser = command.CommitBy;
            if (commitUser == null) throw new ArgumentNullException(nameof(command.CommitBy));

            var status = ApprovalStatus.WaitingApproval;

            var approval = _contentManager.New("Approval");


            var record = new ApprovalPartRecord {
                ContentRecord = content.Record,
                ContentType = content.ContentType,
                CommitBy = command.CommitBy,
                Status = status,
                CommitDate = DateTime.Now,
                ApprovalType = command.ApprovalType,
                OldContentVersion = command.OldContentVersion,
                NewContentVersion = command.NewContentVersion,
                Steps = new List<ApprovalStepRecord> {
                },
            };


            var approvalPart = approval.As<ApprovalPart>();
            // when new a content, maybe auto create the Record ? no, it's null
            //AutoMapper.Mapper.Map(record, approvalPart.Record);
            approvalPart.Record = record;
            //NUnit.Framework.Assert.AreEqual(record.Steps, approvalPart.Steps);
            //approvalPart.SetReferenceContent(content);

            // must create to get ContentItemRecord
            _contentManager.Create(approval, VersionOptions.Published);
            record.ContentItemRecord = approval.Record;

            _approvalPartRecordRepos.Create(record);

            var step = new ApprovalStepRecord() {
                Approval = record,
                Status = status,
            };
            record.CurrentStep = step;
            record.Steps.Add(step);

            var steps = record.Steps;
            NUnit.Framework.Assert.IsNotNull(steps);
            NUnit.Framework.Assert.That(steps.Count > 0);
            steps[0].Status = ApprovalStatus.WaitingApproval;
            foreach (var item in steps) {
                ApprovalStepRepository.Create(step);
            }



            OnCreateApproval(approvalPart);


            return approvalPart;
        }


        ///// <summary>
        ///// 创建审批对象
        ///// </summary>
        ///// <param name="approval">审批对象</param>
        ///// <param name="approvalSwitch">审批开关</param>
        ///// <param name="commitUser">提交用户</param>
        //[Rhythm.Transaction.Transactional]
        //protected virtual void CreateApproval(Orchard.WorkContext wc, ApprovalPart approval, ContentItem content, ApprovalSwitch approvalSwitch, UserPartRecord commitUser) {
        //    if (approval == null) {
        //        throw new ArgumentNullException(nameof(approval));
        //    }
        //    if (commitUser == null) {
        //        throw new ArgumentNullException(nameof(commitUser));
        //    }
        //    approval.Status = ApprovalStatus.WaitingApproval;

        //    approvalRepository.Create(approval);

        //    //var ContentRecord = approval.ContentRecord;
        //    // var content = ContentRecord.GetContentItem(_contentManager);
        //    UpdateCurrentApproval(content, approval);

        //    OnCreateApproval(approval);
        //    var steps = approval.Steps;
        //    NUnit.Framework.Assert.IsNotNull(steps);
        //    NUnit.Framework.Assert.That(steps.Count > 0);
        //    steps[0].Status = ApprovalStatus.WaitingApproval;

        //    // If Not On
        //    if (!approvalSwitch.IsOn()) {
        //        Approve(wc, approval, approvalSwitch, commitUser, commitUser, true);
        //    }
        //}


        #endregion

        ///// <summary>
        ///// 审批驳回
        ///// </summary>
        ///// <param name="approvalId">审批Id</param>
        //public virtual ApprovalPart Reject(Orchard.WorkContext wc, int approvalId) {
        //    return Reject(wc, approvalId, null);
        //}

        /// <summary>
        /// 审批驳回
        /// </summary>
        /// <param name="approvalId">审批Id</param>
        /// <param name="comments">审批意见</param>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Reject(Orchard.WorkContext wc, int approvalId, string comments) {
            var approval = GetApprovalPart(approvalId);
            if (approval == null) {
                throw new ArgumentException(StringUtility.Format("给定的审批#{0}不存在", approvalId), nameof(approvalId));
            }

            //var isAutoApprove = false;
            var approvalSwitch = GetApprovalSwitch();
            var approvalUser = wc.User();

            //    return Reject(wc, approval, GetApprovalSwitch(), approval.CommitBy, wc.CurrentUser.As<UserPart>().Record, comments);
            //}


            ///// <summary>
            ///// 审批驳回
            ///// </summary>
            ///// <param name="approval">审批对象</param>
            ///// <param name="approvalSwitch">审批开关</param>
            ///// <param name="commitUser">提交用户</param>
            ///// <param name="approvalUser">审批用户</param>
            ///// <param name="comments">备注</param>
            ///// <returns>审批对象</returns>
            //[Rhythm.Transaction.Transactional]
            //public virtual ApprovalPart Reject(Orchard.WorkContext wc, ApprovalPart approval, ApprovalSwitch approvalSwitch, UserPartRecord commitUser, UserPartRecord approvalUser, string comments = null) {

            //if (approval == null) {
            //    throw new ArgumentNullException(nameof(approval));
            //}
            //if (commitUser == null) {
            //    throw new ArgumentNullException(nameof(commitUser));
            //}
            if (approvalUser == null) {
                throw new ArgumentNullException(nameof(approvalUser));
            }

            var @event = new ApprovalRejectEvent {
                Approval = approval,
                ApprovalSwitch = approvalSwitch,
                AuditBy = approvalUser,
                AuditOpinion = comments
                //CommitUser = commitUser,
            };

            using (var trace = wc.BeginTrace(@event)) {
                var steps = approval.Steps;
                var currentStep = approval.CurrentStep;

                // validate
                NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
                if (approval.Status != ApprovalStatus.WaitingApproval) {
                    throw ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus.WaitingApproval, approval.Status);
                }
                ValidateCurrentStep(approval, currentStep);
                ValidateDepartment(@event, trace, approvalUser.Department(), currentStep.Department);

                currentStep.AuditOpinion = comments;
                approval.AuditOpinion = comments;

                @event.Step = currentStep;
                currentStep.Status = ApprovalStatus.Rejected;
                ApprovalStepRepository.Update(currentStep);
                approval.CurrentStep = null;

                approval.Status = ApprovalStatus.Rejected;
                approval.AuditDate = DateTime.Now;

                {
                    var contentReflectedType = approval.ContentType;
                    NUnit.Framework.Assert.IsNotNull(contentReflectedType, StringUtility.Format("the Approval#{0}.ContentType is null.", approval.Id));
                    // var contentRepository = RepositoryManager.Default.Of(approval.GetContentRecordType());
                    //var content = approval.ContentRecord;
                    var ContentRecord = approval.ContentRecord;
                    var content = ContentRecord.GetContentItem(_contentManager);
                    var contentApproval = GetContentApprovalSupport(content);
                    NUnit.Framework.Assert.IsNotNull(content);
                    //contentApproval.Status = ApprovalStatus.Rejected;
                    //contentApproval.ApprovalType = approval.ApprovalType;
                    contentApproval.AuditOpinion = comments;
                    UpdateCurrentApproval(contentApproval, approval);
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
        internal static void RejectContentChange(ContentItem content, ApprovalType actionType) {
        }



        //private void UpdateCurrentApproval(ContentItem content, ApprovalPart approval) {
        //    ApprovalSupportPart contentApproval = GetContentApprovalSupport(content);
        //    contentApproval.CurrentApproval = approval.Record;
        //    _approvalSupportRepos.Update(contentApproval.Record);
        //}

        private ApprovalPart GetApprovalPart(int approvalId) {
            // var part =  _contentManager.Get<ApprovalPart>(approvalId);
            return _approvalRepository.Get(approvalId);
        }

        private ApprovalSupportPart GetContentApprovalSupport(ContentItem content) {
            var contentApproval = content.As<ApprovalSupportPart>();
            if (contentApproval.Record == null) {
                var record = _approvalSupportRepos.Table.SingleOrDefault(x => x.ContentItemRecord.Id == content.Id);
                if (record == null) {
                    record = new ApprovalSupportPartRecord() {
                        ContentItemRecord = content.Record,
                    };
                    _approvalSupportRepos.Create(record);
                }
                contentApproval.Record = record;
            }
            return contentApproval;
        }

        private void UpdateCurrentApproval(ApprovalSupportPart contentApproval, ApprovalPart approval) {
            //if (approval.Status == ApprovalStatus.Approved) {
            //    contentApproval.Current = null;
            //}
            //else {
            //    contentApproval.Current = approval.Record;
            //}
            contentApproval.Current = approval.Record;

            contentApproval.Status = approval.Status;
            contentApproval.ApprovalType = approval.ApprovalType;
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
        ///// 批量审批通过
        ///// </summary>
        //public ApprovalPart[] BatchApprove(Orchard.WorkContext wc, int[] approvalIds) {
        //    return Batch(wc, approvalIds, Approve);
        //}


        ///// <summary>
        ///// 批量审批驳回
        ///// </summary>
        //public ApprovalPart[] BatchReject(Orchard.WorkContext wc, int[] approvalIds) {
        //    return Batch(wc, approvalIds, Reject);
        //}

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="approvalId">审批Id</param>
        [Rhythm.Transaction.Transactional]
        public virtual ApprovalPart Approve(Orchard.WorkContext wc, int approvalId, string AuditOpinion) {
            var approval = GetApprovalPart(approvalId);
            if (approval == null) throw new ArgumentException(StringUtility.Format("给定的审批#{0}不存在", approvalId), nameof(approvalId));
            //    return Approve(wc, approval, GetApprovalSwitch(), approval.CommitBy, wc.User());
            //}

            var isAutoApprove = false;
            var approvalSwitch = GetApprovalSwitch();
            var approvalUser = wc.User();


            ///// <summary>
            ///// 审批通过
            ///// </summary>
            ///// <param name="approval">审批对象</param>
            ///// <param name="approvalSwitch">审批开关</param>
            ///// <param name="commitUser">提交用户</param>
            ///// <param name="approvalUser">审批用户</param>
            ///// <param name="isAutoApprove">是否自动审批</param>
            ///// <returns>审批对象</returns>
            //[Rhythm.Transaction.Transactional]
            //public virtual ApprovalPart Approve(Orchard.WorkContext wc, ApprovalPart approval, ApprovalSwitch approvalSwitch, UserPartRecord commitUser, UserPartRecord approvalUser, bool isAutoApprove = false) {
            //if (approval == null) throw new ArgumentNullException(nameof(approval));
            //if (commitUser == null) throw new ArgumentNullException(nameof(commitUser));
            if (approvalUser == null) throw new ArgumentNullException(nameof(approvalUser));

            var steps = approval.Steps;
            var currentStep = approval.CurrentStep;

            var @event = new ApprovalApprove {
                Approval = approval,
                ApprovalSwitch = approvalSwitch,
                AuditBy = approvalUser,
                AuditOpinion = AuditOpinion,
                //CommitUser = commitUser,
                Step = currentStep,
                Date = DateTime.Now,
            };

            NUnit.Framework.Assert.AreNotEqual(0, approval.Id);
            ValidateCurrentStep(approval, currentStep);

            using (var trace = wc.BeginTrace(@event)) {
                if (approval.Status != ApprovalStatus.WaitingApproval) {
                    throw trace.Error(ValidationApprovalCommentsTypeIncorrectError(ApprovalStatus.WaitingApproval, approval.Status));
                }
                ValidateDepartment(@event, trace, approvalUser.Department(), currentStep.Department);

                currentStep.AuditOpinion = AuditOpinion;
                currentStep.Status = ApprovalStatus.Approved;

                approval.AuditOpinion = AuditOpinion;

                // 如果到了最后一级审批
                if (currentStep.Seq + 1 == steps.Count) {
                    approval.CurrentStep = null;
                    SetApprove(approval, approvalUser, isAutoApprove ? autoApprovalDisabledMessage : null);

                    //approvalRepository.Update(approval);
                    // var approval = GetApprovalPart(approvalId);
                    _approvalPartRecordRepos.Update(approval.Record);

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
                //// 发布审批通过事件
                //trace.Success();
            }
            return approval;
        }

        private static void ValidateCurrentStep(IApproval approval, ApprovalStepRecord currentStep) {
            if (approval.Status != ApprovalStatus.WaitingApproval) {
                throw new Exception(string.Format("approval.Status != ApprovalStatus.WaitingApproval, is: {0}", approval.Status));
            }
            if (currentStep == null) {
                throw new Exception("approval.CurrentStep is null");
            }
            if (currentStep.Status != ApprovalStatus.WaitingApproval) {
                throw new Exception("currentStep.Status != ApprovalStatus.WaitingApproval");
            }
        }

        static void ValidateDepartment(Event @event, IDisposable trace, DepartmentRecord userDept, DepartmentRecord stepDept) {
            if (stepDept != null) {
                if (userDept == null) {
                    throw trace.Error(new Exception("您没有所属部门，没有权限进行审批操作"));
                }
                else if (userDept.Id != stepDept.Id) {
                    throw trace.Error(new Exception(StringUtility.Format("您所属部门为 {0}，当前审批进程应由 {1} 进行审批。", userDept.Name, stepDept.Name)));
                }
            }
        }

        /// <summary>
        /// 设置审批对象为上级审批通过
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="comments"></param>
        void SetApprove(ApprovalPart approval, UserPartRecord approvalByUser, string comments = null) {

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

            var contentApproval = GetContentApprovalSupport(content);

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
                // content.IsDeleted = true;
            }
            else {
                throw new InvalidOperationException("unsupported ApprovalType#" + approval.ApprovalType);
            }

            // !!Check
            //contentRepository.Update(content);

            approval.AuditOpinion = comments;
            approval.AuditDate = DateTime.Now;
            approval.AuditBy = approvalByUser;
            approval.Status = ApprovalStatus.Approved;

            //contentApproval.CurrentApproval = null;
            //contentApproval.Status = ApprovalStatus.Approved;
            //contentApproval.ApprovalType = approval.ApprovalType;
            UpdateCurrentApproval(contentApproval, approval);

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

        ///// <summary>
        ///// 验证被审批的内容的状态是否合法
        ///// </summary>
        ///// <param name="content"></param>
        ///// <param name="actionType"></param>
        //void SetContentWaitingApproval(Orchard.WorkContext wc, ContentItem content, System.Type actionType) {
        //    NUnit.Framework.Assert.IsNotNull(content.As<CommonPart>().Owner, "content.CreatedByUser is null");
        //    NUnit.Framework.Assert.IsNotNull(wc.User(), "Orchard.WorkContext.User is null");


        //    var contentApproval = GetContentApprovalSupport(content);

        //    //if (content.ApprovalStatus == ApprovalStatus.Deleted)
        //    //if (content.IsDeleted) {
        //    //    throw ValidationError("该内容已被删除，不能提交审批。");
        //    //}
        //    if (contentApproval.Status == ApprovalStatus.WaitingApproval) {
        //        throw ValidationError("该内容已经提交审批，不能重复提交。");
        //    }
        //    contentApproval.Status = ApprovalStatus.WaitingApproval;
        //    contentApproval.ApprovalType = actionType;

        //    // !!Check
        //    //RepositoryManager.Default.Of(content.GetContentRecordType()).Update(content);
        //}

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
        public bool ShoultAotoCommit(UserPartRecord user) {
            return ShoultAotoCommit(user, GetApprovalSwitch());
        }

        /// <summary>
        /// 自动提交
        /// </summary>
        /// <param name="approvalSwitch">审批开关</param>
        /// <param name="user">用户对象</param>
        /// <returns>是或者否</returns>
        public bool ShoultAotoCommit(UserPartRecord user, ApprovalSwitch approvalSwitch) {
            return false;
        }


        #endregion



    }
}
