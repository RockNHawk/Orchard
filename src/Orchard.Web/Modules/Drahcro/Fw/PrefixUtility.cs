using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Layouts.Framework.Drivers;

namespace Drahcro {
    public class PrefixUtility {

        public static string GetPrefix(ElementEditorContext context, string name) {
            return $"{(string.IsNullOrEmpty(context.Prefix) ? null : ".")}{name}";
        }

        public static string GetPrefix(string prefix, string name) {
            return $"{(string.IsNullOrEmpty(prefix) ? null : ".")}{name}";
        }


    }
}