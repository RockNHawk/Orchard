using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Contrib.Dashboard.Models
{
    public class DashboardItemRecord {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Position { get; set; }
        public virtual string Category { get; set; }
        public virtual string Type { get; set; }
        public virtual string Parameters { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string Name { get; set; }
    }
}