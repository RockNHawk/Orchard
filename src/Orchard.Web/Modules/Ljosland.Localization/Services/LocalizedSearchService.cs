using System;
using System.Linq;
using Orchard.Collections;
using Orchard.Indexing;
using Orchard.Localization;
using Orchard.Localization.Services;
using System.Globalization;
using Orchard.Search.Services;
using Orchard;
using Orchard.Environment.Extensions;

namespace Ljosland.Localization.Search.Services
{
    [OrchardFeature("Localized.Search")]
    [OrchardSuppressDependency("Orchard.Search.Services.SearchService")]
    public class LocalizedSearchService : ISearchService {
        private readonly IIndexManager _indexManager;
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizedSearchService(IOrchardServices services, IIndexManager indexManager, ICultureManager cultureManager, IWorkContextAccessor workContextAccessor)
        {
            Services = services;
            _indexManager = indexManager;
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        ISearchBuilder Search() {
            return _indexManager.HasIndexProvider()
                ? _indexManager.GetSearchIndexProvider().CreateSearchBuilder("Search")
                : new NullSearchBuilder();
        }

        IPageOfItems<T> ISearchService.Query<T>(string query, int page, int? pageSize, bool filterCulture, string[] searchFields, Func<ISearchHit, T> shapeResult) {

            if (string.IsNullOrWhiteSpace(query))
                return new PageOfItems<T>(Enumerable.Empty<T>());

            var searchBuilder = Search().Parse(searchFields, query);

            //if (filterCulture) {
                // TODO: (sebros) Implementations details after Alpha
                //var culture = _cultureManager.GetSiteCulture();
                var culture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);

                // use LCID as the text representation gets analyzed by the query parser
                searchBuilder
                    .WithField("culture", CultureInfo.GetCultureInfo(culture).LCID)
                    .AsFilter();
                 
            //}

            var totalCount = searchBuilder.Count();
            if (pageSize != null)
                searchBuilder = searchBuilder
                    .Slice((page > 0 ? page - 1 : 0) * (int)pageSize, (int)pageSize);

            var searchResults = searchBuilder.Search();

            var pageOfItems = new PageOfItems<T>(searchResults.Select(shapeResult)) {
                PageNumber = page,
                PageSize = pageSize != null ? (int)pageSize : totalCount,
                TotalItemCount = totalCount
            };

            return pageOfItems;
        }
    }
}