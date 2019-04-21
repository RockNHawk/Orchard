using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Blogs.Services;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.Blogs.Routing;
using Orchard.Blogs.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization.Services;
using Orchard;
using Orchard.Data;
using Orchard.Tasks.Scheduling;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Localization.Models;
using Orchard.ContentManagement.Records;
using Orchard.Localization.Records;

namespace Ljosland.Localization.Services
{
    [OrchardFeature("Localized.Blogs")]
    [OrchardSuppressDependency("Orchard.Blogs.Services.BlogPostService")]
    public class LocalizedBlogPostService : IBlogPostService
    {
        private readonly IContentManager _contentManager;
        private readonly IRepository<BlogPartArchiveRecord> _blogArchiveRepository;
        private readonly IPublishingTaskManager _publishingTaskManager;
        private ICultureManager _cultureManager;
        private IWorkContextAccessor _workContextAccessor;

        public LocalizedBlogPostService(IWorkContextAccessor workContextAccessor, ICultureManager cultureManager, IContentManager contentManager, IRepository<BlogPartArchiveRecord> blogArchiveRepository, IPublishingTaskManager publishingTaskManager, IContentDefinitionManager contentDefinitionManager)
        {
            _cultureManager = cultureManager;
            _contentManager = contentManager;
            _blogArchiveRepository = blogArchiveRepository;
            _publishingTaskManager = publishingTaskManager;
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<BlogPostPart> Get(BlogPart blogPart, VersionOptions versionOptions)
        {
            return GetBlogQuery(blogPart, versionOptions).List().Select(ci => ci.As<BlogPostPart>());
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetBlogQuery(BlogPart blog, VersionOptions versionOptions)
        {
            //var currentCulture = _cultureManager.GetCultureByName(_cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext));
            var currentBlogLocalization = _contentManager.Query("Blog")
                .Join<LocalizationPartRecord>()
                .Where(x => x.Id == blog.ContentItem.Id).List<LocalizationPart>().SingleOrDefault();

            //also look in current selected blog
            var blogids = new HashSet<int> { blog.ContentItem.Record.Id };

            //and look in master blog if current blog is a translation
            _contentManager.Query("Blog")
                .Join<LocalizationPartRecord>()
                .Where(x => x.MasterContentItemId == blog.ContentItem.Id)
                .List().ToList().ForEach(x => blogids.Add(x.Id));

            //and look in all master blog's translations
            if (currentBlogLocalization != null && currentBlogLocalization.MasterContentItem != null)
            {
                _contentManager.Query("Blog").Join<CommonPartRecord>().Where(x => x.Id == currentBlogLocalization.MasterContentItem.Id).List().ToList().ForEach(x => blogids.Add(x.Id));
            }

            if (currentBlogLocalization != null)
            {
                return _contentManager.Query(versionOptions, "BlogPost")
                    .Join<LocalizationPartRecord>().Where(x => x.CultureId == currentBlogLocalization.Culture.Id)     
                    .Join<CommonPartRecord>().Where(cr => blogids.ToList().Contains(cr.Container.Id))                       
                    .OrderByDescending(cr => cr.CreatedUtc).WithQueryHintsFor("BlogPost");
            }

            return _contentManager.Query(versionOptions, "BlogPost")
                        .Join<CommonPartRecord>().Where(cr => blogids.ToList().Contains(cr.Container.Id))
               .OrderByDescending(cr => cr.CreatedUtc).WithQueryHintsFor("BlogPost");
        }

        public BlogPostPart Get(int id)
        {
            return Get(id, VersionOptions.Published);
        }

        public BlogPostPart Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get<BlogPostPart>(id, versionOptions);
        }

        public IEnumerable<BlogPostPart> Get(BlogPart blogPart)
        {
            return Get(blogPart, VersionOptions.Published);
        }

        public IEnumerable<BlogPostPart> Get(BlogPart blogPart, ArchiveData archiveData)
        {
            var query = GetBlogQuery(blogPart, VersionOptions.Published);

            if (archiveData.Day > 0)
            {
                var dayDate = new DateTime(archiveData.Year, archiveData.Month, archiveData.Day);

                query = query.Where(cr => cr.CreatedUtc >= dayDate && cr.CreatedUtc < dayDate.AddDays(1));
            }
            else if (archiveData.Month > 0)
            {
                var monthDate = new DateTime(archiveData.Year, archiveData.Month, 1);

                query = query.Where(cr => cr.CreatedUtc >= monthDate && cr.CreatedUtc < monthDate.AddMonths(1));
            }
            else
            {
                var yearDate = new DateTime(archiveData.Year, 1, 1);

                query = query.Where(cr => cr.CreatedUtc >= yearDate && cr.CreatedUtc < yearDate.AddYears(1));
            }

            return query.List().Select(ci => ci.As<BlogPostPart>());
        }

        public IEnumerable<BlogPostPart> Get(BlogPart blogPart, int skip, int count)
        {
            return Get(blogPart, skip, count, VersionOptions.Published);
        }

        public IEnumerable<BlogPostPart> Get(BlogPart blogPart, int skip, int count, VersionOptions versionOptions)
        {
            return GetBlogQuery(blogPart, versionOptions)
                    .Slice(skip, count)
                    .ToList()
                    .Select(ci => ci.As<BlogPostPart>());
        }

        public int PostCount(BlogPart blogPart)
        {
            return PostCount(blogPart, VersionOptions.Published);
        }

        public int PostCount(BlogPart blogPart, VersionOptions versionOptions)
        {
            return GetBlogQuery(blogPart, versionOptions).Count();
        }

        public IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives(BlogPart blogPart)
        {
            var query =
                from bar in _blogArchiveRepository.Table
                where bar.BlogPart == blogPart.ContentItem.Record
                orderby bar.Year descending, bar.Month descending
                select bar;

            return
                query.ToList().Select(
                    bar =>
                    new KeyValuePair<ArchiveData, int>(new ArchiveData(string.Format("{0}/{1}", bar.Year, bar.Month)),
                                                       bar.PostCount));
        }

        public void Delete(BlogPostPart blogPostPart)
        {
            _publishingTaskManager.DeleteTasks(blogPostPart.ContentItem);
            _contentManager.Remove(blogPostPart.ContentItem);
        }

        public void Publish(BlogPostPart blogPostPart)
        {
            _publishingTaskManager.DeleteTasks(blogPostPart.ContentItem);
            _contentManager.Publish(blogPostPart.ContentItem);
        }

        public void Publish(BlogPostPart blogPostPart, DateTime scheduledPublishUtc)
        {
            _publishingTaskManager.Publish(blogPostPart.ContentItem, scheduledPublishUtc);
        }

        public void Unpublish(BlogPostPart blogPostPart)
        {
            _contentManager.Unpublish(blogPostPart.ContentItem);
        }

        public DateTime? GetScheduledPublishUtc(BlogPostPart blogPostPart)
        {
            var task = _publishingTaskManager.GetPublishTask(blogPostPart.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }
    }
}