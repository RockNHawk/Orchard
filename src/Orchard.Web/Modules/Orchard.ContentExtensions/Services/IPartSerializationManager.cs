using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchard.ContentExtensions.Services
{
    public interface IPartSerializationManager : ISingletonDependency
    {
        object Convert(ContentPart part);

        object Convert(ContentItem contentItem);
    }
}
