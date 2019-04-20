using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MnLab.Enterprise.Approval.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Security;
using Orchard.Users.Models;
using Rhythm.Diagnostics;
using Rhythm.EventSourcing;

namespace Rhythm {

    /// <summary>
    /// </summary>
    public static class OrchardUserExtension {

        public static DepartmentRecord Department(this UserPartRecord user1) {
            return null;
        }

    }
}