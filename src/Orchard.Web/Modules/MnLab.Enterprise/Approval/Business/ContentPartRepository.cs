

using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace MnLab.Enterprise {
    public class ContentPartRepository<TContentPart, TRecord>
        : IDependency, IContentPartRepository<TContentPart, TRecord>
        where TContentPart : ContentPart<TRecord>
        where TRecord : ContentPartRecord {
        IContentManager _contentManager;
        IRepository<TRecord> repository;
        public ContentPartRepository(IContentManager contentManager, IRepository<TRecord> repository) {
            this._contentManager = contentManager;
            this.repository = repository;
        }

        public TContentPart Get(int id) {
            var part = _contentManager.Get<TContentPart>(id);
            if (part.Record == null) {
                part.Record = repository.Get(x => x.ContentItemRecord.Id == id);
            }
            return part;
        }

        public void Update(TContentPart obj) {
            repository.Update(obj.Record);
        }

        public void Create(TContentPart obj) {
            obj.Record.ContentItemRecord = obj.ContentItem.Record;
            repository.Create(obj.Record);
        }
    }
}
