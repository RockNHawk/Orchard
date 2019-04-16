using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ContentExtensions.Services
{
    public interface IExtendedContentManager : IDependency
    {
        ContentItem Get(int id, string contentType);
        ContentItem Get(int id, VersionOptions options, string contentType);
        ContentItem Get(int id, VersionOptions options, QueryHints hints, string contentType);
    }
}