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
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using Rhythm;
using Drahcro.Data;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using MnLab.Enterprise.Approval.Models;
using Orchard.Events;

namespace MnLab.Enterprise.Approval.Events {

    /// <summary>
    /// http://www.ideliverable.com/blog/ieventhandler
    /// </summary>
    public interface IApprovalEventHandler : IEventHandler {
        void Commit(CreateApprovalCommand command);
        void Approve(ApprovalApproveCommand command);
        void Reject(ApprovalRejectCommand command);
    }
}