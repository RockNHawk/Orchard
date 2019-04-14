using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Security;

namespace Rhythm {
    /// <summary>
    /// </summary>
    public static class OrchardUserExtension {


        public static string Username(this IUser user1) {
            var userContent = user1.ContentItem;

            /// http://derekdevdude.blogspot.com/2012/10/accessing-orchard-current-user-content.html
            //userContent.As<ProfilePart>();
            //if (userContent.ProfilePart != null &&
            //  userContent.ProfilePart.Has(typeof(object), "DisplayName") &&
            //  userContent.ProfilePart.DisplayName.Value is string) {
            //    displayName = userContent.ProfilePart.DisplayName.Value.Trim();
            //}
            //if (String.IsNullOrWhiteSpace(displayName)) {

            return user1.UserName;

        }
    }
}