using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System;
namespace Orchard.ContentExtensions.Services
{
    public interface IPartTypeRecordMatchingService : ISingletonDependency
    {
        void Set(ContentPart part, object record);
        bool Match(ContentPart part, object record);
    }
}
