using Orchard.ContentManagement;

namespace MnLab.Enterprise {
    public interface IContentPartRepository<TContentPart, TRecord> where TContentPart : ContentPart<TRecord> {
        void Create(TContentPart obj);
        TContentPart Get(int id);
        void Update(TContentPart obj);
    }
}