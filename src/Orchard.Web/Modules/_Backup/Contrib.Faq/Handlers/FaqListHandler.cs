using Orchard.ContentManagement.Handlers;
using Contrib.Faq.Models;
using Orchard.Data;

namespace Contrib.Faq.Handlers {
  public class FaqListHandler : ContentHandler {
    public FaqListHandler(IRepository<FaqListRecord> faqlistrepository) {
      Filters.Add(StorageFilter.For(faqlistrepository));
    }
  }
}
