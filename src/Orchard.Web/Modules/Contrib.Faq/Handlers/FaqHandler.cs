using Orchard.ContentManagement.Handlers;
using Contrib.Faq.Models;
using Orchard.Data;

namespace Contrib.Faq.Handlers {
  public class FaqHandler : ContentHandler {
    public FaqHandler(IRepository<FaqPartRecord> faqrepository) {
      Filters.Add(StorageFilter.For(faqrepository));
    }
  }
}
