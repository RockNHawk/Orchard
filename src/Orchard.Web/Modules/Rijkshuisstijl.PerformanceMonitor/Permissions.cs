using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Rijkshuisstijl.PerformanceMonitor
{
    public class Permissions : IPermissionProvider
    {
        public const string ConfigureCounterPermission = "Configure performance counter";

        public static readonly Permission ConfigurePerformanceMonitorCounter = new Permission
        {
            Description = "Show and edit the settings for the performance monitor",
            Name = ConfigureCounterPermission
        };

        public Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] { ConfigurePerformanceMonitorCounter };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[]
                    {
                        ConfigurePerformanceMonitorCounter
                    }
                }
            };
        }

    }
}