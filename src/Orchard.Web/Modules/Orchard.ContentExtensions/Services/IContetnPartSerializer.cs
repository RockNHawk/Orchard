using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchard.ContentExtensions.Services
{
    public interface IContetnPartSerializer : ISingletonDependency
    {
        string ForContentPart { get; }
        object GetSerializableObject(ContentPart contentPart);
    }
}
