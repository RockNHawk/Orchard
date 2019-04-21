using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Widgets.Services;

namespace CyberStride.TypeLayerRule {
    public class ContentTypeRuleProvider : IRuleProvider {
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWorkContextAccessor _workContextAccessor;

		public ContentTypeRuleProvider(IHttpContextAccessor httpContextAccessor, IWorkContextAccessor workContextAccessor) {
		    _httpContextAccessor = httpContextAccessor;
		    _workContextAccessor = workContextAccessor;
		}
        public void Process(RuleContext ruleContext) {
            if (!String.Equals(ruleContext.FunctionName, "type", StringComparison.OrdinalIgnoreCase) ||
                ruleContext.Arguments[0] == null ||
                String.IsNullOrWhiteSpace(ruleContext.Arguments[0].ToString()))
            {
                return;
            }
            ruleContext.Result = false;
            var typeParam = ruleContext.Arguments[0].ToString();
            var contentItems = _workContextAccessor.GetContext().GetState<List<IContent>>("ContentItems");
            if(contentItems!= null && contentItems.Count > 0) 
            {
                foreach (var content in contentItems) 
                {
                    var contentType = content.ContentItem != null ? content.ContentItem.ContentType : string.Empty;
                    if (contentType.Equals(typeParam, StringComparison.CurrentCultureIgnoreCase)) 
                        ruleContext.Result = true;
                }
            }
        }
    }
}