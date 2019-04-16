using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Orchard.ContentExtensions.ViewModels
{
    public class AjaxMessageViewModel
    {
        private Collection<string> errors = new Collection<string>();

        public int Id { get; set; }

        public bool IsDone { get; set; }

        public Collection<string> Errors
        {
            get
            {
                return this.errors;
            }
        }

        public string Html { get; set; }

        public object Data { get; set; }
    }
}