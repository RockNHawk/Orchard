//using Orchard.ContentManagement;
//using Orchard.ContentManagement.Aspects;
//using Orchard.ContentManagement.Handlers;
//using MnLab.Approval.Models;
//using Orchard.Data;

//namespace MnLab.Approval.Handlers {
//    public class ApprovalPartHandler : ContentHandler {

//        public ApprovalPartHandler(IRepository<ApprovalPartRecord> repository) {
//            Filters.Add(StorageFilter.For(repository));
//        //    OnIndexing<ITitleAspect>((context, part) => context.DocumentIndex.Add("title", part.Title).RemoveTags().Analyze());
//        }

//        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
//            var part = context.ContentItem.As<ITitleAspect>();

//            if (part != null) {
//                context.Metadata.DisplayText = part.Title;
//            }
//        }
//    }
//}
