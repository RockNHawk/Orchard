using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Orchard.Contrib.Dashboard.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace Orchard.Contrib.Dashboard.ImportExport {
    public class DashboardRecipeHandler : IRecipeHandler {
        private readonly IDashboardItemsService _dashboardItemsServices;

        public DashboardRecipeHandler(IDashboardItemsService dashboardItemsServices)
        {
            _dashboardItemsServices = dashboardItemsServices;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        // <Data />
        // Import Data
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "Dashboard", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            foreach (var element in recipeContext.RecipeStep.Step.Elements()) {

                var record = _dashboardItemsServices.CreateItem(element.Attribute("Name").Value);

                record.Enabled = bool.Parse(element.Attribute("Enabled").Value);
                record.Type = element.Attribute("Type").Value;
                record.Category = element.Attribute("Category").Value;
                record.Parameters = element.Attribute("Parameters").Value;
                record.Position = int.Parse(element.Attribute("Position").Value);
            }

            recipeContext.Executed = true;
        }
    }
}
