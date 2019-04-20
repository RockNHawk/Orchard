

using Autofac;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Drahcro.Data {

    public class ContentPartRecordRepository : IDependency {
        IContentManager _contentManager;
        IWorkContextAccessor container;
        public ContentPartRecordRepository(IContentManager contentManager, IWorkContextAccessor container) {
            this._contentManager = contentManager;
            this.container = container;
        }

        public TContentPart Get<TContentPart, TRecord>(int id)
            where TContentPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord {
            var part = _contentManager.Get<TContentPart>(id);
            if (part.Record == null) {
                var repository = container.GetContext().Resolve<IRepository<TRecord>>();
                part.Record = repository.Get(x => x.ContentItemRecord.Id == id);
            }
            return part;
        }


        public TRecord Fill<TRecord>(ContentPart<TRecord> part)
          where TRecord : ContentPartRecord {
            if (part.Record == null) {
                var repository = container.GetContext().Resolve<IRepository<TRecord>>();
                part.Record = repository.Get(x => x.ContentItemRecord.Id == part.Id);
            }
            return part.Record;
        }


        //public TContentPart Fill<TContentPart, TRecord>(TContentPart part)
        //    where TContentPart : ContentPart<TRecord>
        //    where TRecord : ContentPartRecord {
        //    if (part.Record == null) {
        //        var repository = container.Resolve<IRepository<TRecord>>();
        //        part.Record = repository.Get(x => x.ContentItemRecord.Id == part.Id);
        //    }
        //    return part;
        //}


        //public TRecord Get<TRecord>(ContentPart<TRecord> part)
        //   where TRecord : ContentPartRecord {
        //    var part = _contentManager.Get<TContentPart>(id);
        //    if (part.Record == null) {
        //        var repository = container.Resolve<IRepository<TRecord>>();
        //        part.Record = repository.Get(x => x.ContentItemRecord.Id == id);
        //    }
        //    return part;
        //}

    }

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
