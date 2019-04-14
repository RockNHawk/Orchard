//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace YueQing.Lend
//{
//    public class Approval<TContent, TContentVersion> : Approval
//        where TContent : IContent
//        where TContentVersion : IContentVersion
//    {
//        public TContent Content { get; set; }
//        public TContentVersion NewVersion { get; set; }
//        public TContentVersion OldVersion { get; set; }
//    }

//    public class CreationApproval<TContent, TContentVersion> : Approval<TContent, TContentVersion> { }
//    public class ModificationApproval<TContent, TContentVersion> : Approval<TContent, TContentVersion> { }
//    public class DeletionApproval<TContent, TContentVersion> : Approval<TContent, TContentVersion> { }

//    public class ArticleCreationApproval : CreationApproval<Article, ArticleVersion> { }
//    public class ArticleModificationApproval : ModificationApproval<Article, ArticleVersion> { }
//    public class ArticleDeletionApproval : DeletionApproval<Article, ArticleVersion> { }

//}