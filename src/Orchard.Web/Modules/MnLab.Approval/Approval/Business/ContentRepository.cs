

using Orchard;
using Orchard.ContentManagement;

namespace MnLab.Enterprise.Approval {
    public class ContentRepository<T> : IDependency where T : IContent {
        IContentManager _contentManager;
        public ContentRepository(IContentManager contentManager) {
            this._contentManager = contentManager;
        }

        public T Get(int id) {
            return _contentManager.Get(id).As<T>();
        }

        public void Update(T obj) {
        }

        public void Save(T obj) {
        }
    }
}
