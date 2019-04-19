using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Drivers.Coordinators;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Drivers;
using Orchard.Core.Common.Handlers;
using Orchard.Core.Common.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.OwnerEditor;
using Orchard.Core.Common.Services;
using Orchard.Core.Scheduling.Models;
using Orchard.Core.Scheduling.Services;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Descriptors.ShapeAttributeStrategy;
using Orchard.DisplayManagement.Descriptors.ShapePlacementStrategy;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.VirtualPath;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Tasks.Scheduling;
using Orchard.Tests.DisplayManagement;
using Orchard.Tests.DisplayManagement.Descriptors;
using Orchard.Tests.Modules;
using System.Web.Mvc;
using Orchard.Tests.Stubs;
using Orchard.Themes;
using Orchard.UI.PageClass;
using Orchard.Core.Tests.Common.Providers;

using Rhythm;
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
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;


namespace MnLab.Enterprise.Tests {



    [TestFixture]
    public class ApprovalTests : DatabaseEnabledTestsBase {

        [Test]
        public void Approval() {

            var contentManager = _container.Resolve<IContentManager>();
            var userRepos = _container.Resolve<IRepository<UserPartRecord>>();

            var approvalService = _container.Resolve<ContentApprovalService>();

            var user = userRepos.Get(x => x.UserName == "admin");

            var content = contentManager.New("NHProductManual");

            Moq.Mock<WorkContext> wk = new Mock<WorkContext>();
            wk.SetupProperty(x => x.CurrentUser, contentManager.Get<UserPart>(user.Id));

            var approval = approvalService.CommmitApproval(wk.Object, new CreateApprovalCommand {
                ContentItem = content,
                ApprovalType = ApprovalType.Creation,
                //CommitBy=wk.Object.CurrentUser
            });


            approvalService.Approve(wk.Object,approval.Id);


            ClearSession();

            //Assert.That(content.Owner, Is.Null);
            //Assert.That(content.Record.OwnerId, Is.EqualTo(0));
        }





        public override void Register(ContainerBuilder builder) {
            new CommonPartProviderTests().Register(builder);
        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                return new[] {
                typeof(UserPartRecord),

                                 typeof(ContentTypeRecord),
                                 typeof(ContentItemRecord),
                                 typeof(ContentItemVersionRecord),
                                 typeof(CommonPartRecord),
                                 typeof(CommonPartVersionRecord),
                                 typeof(ScheduledTaskRecord),
                             };
            }
        }





    }
}
