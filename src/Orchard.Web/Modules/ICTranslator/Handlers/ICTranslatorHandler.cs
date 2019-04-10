using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using ICTranslator.Models;

namespace ICTranslator.Handlers
{
    public class MapHandler : ContentHandler {
        public MapHandler(IRepository<ICTranslatorRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
