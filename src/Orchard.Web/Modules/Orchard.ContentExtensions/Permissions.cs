using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ContentExtensions
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ViewContentTypeExtensions = new Permission { Name = "ViewContentTypeExtensions", Description = "View content types." };
        public static readonly Permission EditContentTypeExtensions = new Permission { Name = "EditContentTypeExtensions", Description = "Edit content types." };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ViewContentTypeExtensions,
                EditContentTypeExtensions
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = GetPermissions()
                }
            };
        }
    }
}