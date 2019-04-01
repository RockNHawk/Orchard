using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;
using System.Linq;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.FileSystems.VirtualPath;
using Orchard;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Zones;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Orchard.Core.Title.Models;

namespace MnLab.PdfVisualDesign.Binding.Drivers
{
public static class ContentItemExtensions {

        public static ContentItem GetLatestVersion(this IContent content, IContentManager _contentManager) {
            var contentItem = content.ContentItem;
            if (content.Id > 0) {
                /*
               context.Content's Version is the publish version, if the content never Published, the field will be null.
               so I get the latest (if have draft) ContentItem from ContentManager
                */
                contentItem = _contentManager.Get(content.Id, VersionOptions.Latest);
            }
            return contentItem;
        }
    }
}