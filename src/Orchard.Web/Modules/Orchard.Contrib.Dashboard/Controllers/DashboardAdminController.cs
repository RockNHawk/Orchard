using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.Contrib.Dashboard.Models;
using Orchard.Contrib.Dashboard.Services;
using Orchard.Contrib.Dashboard.ViewModels;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System.Web.Routing;

namespace Orchard.Contrib.Dashboard.Controllers {
    [Admin,ValidateInput(false)]
    public class DashboardAdminController : Controller {

        public IOrchardServices Services { get; set; }
        private readonly IFormManager _formManager;
        public Localizer T { get; set; }
        public dynamic Shape { get; set; }
        private readonly IDashboardItemsManager _dashboardItemsManager;
        private readonly IDashboardItemsService _dashboardItemsService;
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;

        public DashboardAdminController(
            IOrchardServices services,
            IFormManager formManager,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices,
            IDashboardItemsManager dashboardItemsManager,
            IDashboardItemsService dashboardItemsService,
            ISiteService siteService)
        {
            Services = services;
            _formManager = formManager;
            _orchardServices = orchardServices;
            _dashboardItemsManager = dashboardItemsManager;
            _dashboardItemsService = dashboardItemsService;
            _siteService = siteService;
            Shape = shapeFactory;
        }

        public ActionResult Index(DashboardItemIndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to dashboard items")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            // default options
            if (options == null)
                options = new DashboardItemIndexOptions();

            var allItems = _dashboardItemsService.GetAllItems();

            switch (options.Filter)
            {
                case DashboardItemFilter.Disabled:
                    allItems = allItems.Where(r => r.Enabled == false);
                    break;
                case DashboardItemFilter.Enabled:
                    allItems = allItems.Where(u => u.Enabled);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(options.Search))
            {
                allItems = allItems.Where(r => r.Type.Contains(options.Search));
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(allItems.Count());
            allItems = allItems.OrderBy(o => o.Position);
            switch (options.Order)
            {
                case DashboardItemOrder.Type:
                    allItems = allItems.OrderBy(u => u.Type);
                    break;
                case DashboardItemOrder.Category:
                    allItems = allItems.OrderBy(u => u.Category);
                    break;
            }

            var results = allItems
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();

            var model = new DashboardItemIndexViewModel
            {
                Items = results.Select(x => new DashboardItemEntry
                {
                    Record = x,
                    IsChecked = false,
                    Id = x.Id
                }).ToList(),
                Options = options,
                Pager = pagerShape
            };

            // maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            pagerShape.RouteData(routeData);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to  manage dashboard items")))
                return new HttpUnauthorizedResult();

            var viewModel = new DashboardItemIndexViewModel { Items = new List<DashboardItemEntry>(), Options = new DashboardItemIndexOptions() };
            UpdateModel(viewModel);

            var checkedEntries = viewModel.Items.Where(c => c.IsChecked);
            switch (viewModel.Options.BulkAction)
            {
                case DashboardItemBulkAction.None:
                    break;
                case DashboardItemBulkAction.Enable:
                    foreach (var entry in checkedEntries)
                    {
                        _dashboardItemsService.GetItem(entry.Id).Enabled = true;
                    }
                    break;
                case DashboardItemBulkAction.Disable:
                    foreach (var entry in checkedEntries)
                    {
                        _dashboardItemsService.GetItem(entry.Id).Enabled = false;
                    }
                    break;
                case DashboardItemBulkAction.Delete:
                    foreach (var entry in checkedEntries)
                    {
                        _dashboardItemsService.DeleteItem(entry.Id);
                    }
                    break;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Add()
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to  manage dashboard items")))
                return new HttpUnauthorizedResult();

            var viewModel = new DashboardItemAddViewModel 
                { Items = _dashboardItemsManager.DescribeItems() };
            return View(viewModel);
        }
        public ActionResult Create(string category, string type) 
        {
            var item = _dashboardItemsManager.DescribeItems().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == category && x.Type == type);
            if (item == null)
            {
                return HttpNotFound();
            }
            var record = _dashboardItemsService.CreateItem(type+" "+category,
                Services.WorkContext.CurrentUser.Id);
            record.Type = type;
            record.Category = category;

            return RedirectToAction("Edit", new{Id=record.Id});
        }

        public ActionResult MoveUp(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage dashboard items")))
                return new HttpUnauthorizedResult();

            _dashboardItemsService.MoveUp(id);
            return RedirectToAction("Index");
        }

        public ActionResult MoveDown(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage dashboard items")))
                return new HttpUnauthorizedResult();

            _dashboardItemsService.MoveDown(id);
            return RedirectToAction("Index");
        }

        public ActionResult Enable(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage dashboard items")))
                return new HttpUnauthorizedResult();

            var record = _dashboardItemsService.GetItem(id);
            if (record == null) return HttpNotFound();

            record.Enabled = true;
            return RedirectToAction("Index");
        }
        public ActionResult Disable(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to  manage dashboard items")))
                return new HttpUnauthorizedResult();

            var record = _dashboardItemsService.GetItem(id);
            if (record == null) return HttpNotFound();

            record.Enabled = false;
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to  manage dashboard items")))
                return new HttpUnauthorizedResult();

            var record = _dashboardItemsService.GetItem(id);
            if (record == null) return HttpNotFound();

            var item = _dashboardItemsManager.DescribeItems().SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == record.Category && x.Type == record.Type);
            if (item == null) return HttpNotFound();

            // if there is no form to edit, save the action and go back
            if (item.Form == null)
            {
                return RedirectToAction("Index");
            }

            // build the form, and let external components alter it
            var form = _formManager.Build(item.Form);

            // add a submit button to the form
            AddSubmitButton(form);

            // bind form with existing values).
            var parameters = FormParametersHelper.FromString(record.Parameters);
            _formManager.Bind(form, new DictionaryValueProvider<string>(parameters, CultureInfo.InvariantCulture));

            var viewModel = new DashboardItemEditViewModel { Id = id, Item = item, Form = form };
            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(int id, FormCollection formCollection)
        {
            var record = _dashboardItemsService.GetItem(id);
            if (record == null) return HttpNotFound(); 
            
            var item = _dashboardItemsManager.DescribeItems().SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == record.Category && x.Type == record.Type);

            // validating form values
            _formManager.Validate(new ValidatingContext { FormName = item.Form, ModelState = ModelState, ValueProvider = ValueProvider });

            if (ModelState.IsValid) {
                var dictionary = formCollection.AllKeys.ToDictionary(key => key, formCollection.Get);
                // save form parameters
                record.Parameters = FormParametersHelper.ToString(dictionary);
                if (dictionary.ContainsKey("displayName")) {
                    record.Name = dictionary["displayName"];
                }
                return RedirectToAction("Index");
            }

            // model is invalid, display it again
            var form = _formManager.Build(item.Form);

            // Cancel the current transaction to prevent records from begin created
            Services.TransactionManager.Cancel();

            AddSubmitButton(form);

            _formManager.Bind(form, formCollection);
            var viewModel = new DashboardItemEditViewModel { Id = id, Item = item, Form = form };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage items")))
                return new HttpUnauthorizedResult();

            _dashboardItemsService.DeleteItem(id);
            Services.Notifier.Information(T("Action Deleted"));

            return RedirectToAction("Index");
        }


        private void AddSubmitButton(dynamic form)
        {
            var viewContext = new ViewContext { HttpContext = HttpContext, Controller = this };

            //<=1.5.1
            //var siteSalt = Services.WorkContext.CurrentSite.SiteSalt;
            //var token = new HtmlHelper(viewContext, new ViewDataContainer()).AntiForgeryToken(siteSalt);

            //>1.5.1
            var token = new HtmlHelper(viewContext, new ViewDataContainer()).AntiForgeryToken();


            form
                ._Actions(Shape.Fieldset(
                    _RequestAntiForgeryToken: Shape.Markup(
                        Value: token.ToString()),
                    _Save: Shape.Submit(
                        Name: "op",
                        Value: T("Save"))
                    )
                );
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}