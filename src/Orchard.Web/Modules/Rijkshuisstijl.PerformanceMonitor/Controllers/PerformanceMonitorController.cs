#region Usings

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.UI.Admin;
//using Rijkshuisstijl.ApplicationFramework.Authorization;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.WorkerServices;

#endregion

namespace Rijkshuisstijl.PerformanceMonitor.Controllers
{
    [Admin]
    public class PerformanceMonitorController : Controller
    {
        private readonly IPerformanceMonitorWorkerService _performanceMonitorWorkerService;

        public string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public PerformanceMonitorController(IPerformanceMonitorWorkerService performanceWorkerService)
        {
            _performanceMonitorWorkerService = performanceWorkerService;
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult Index()
        {
            IndexViewModel viewModel = _performanceMonitorWorkerService.Index();
            return View(viewModel);
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult Category(CategoryInputModel inputModel)
        {
            CategoryViewModel viewModel = _performanceMonitorWorkerService.Category(inputModel);
            
            if (viewModel.Accessible == false)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Category")]
        //[HandleError, OrchardAuthorization(PermissionName = ApplicationFrameworkPermissionStrings.EditConfigurationSettings)]
        public ActionResult CategoryPost(CategoryPostInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                CategoryInputModel catInputModel = new CategoryInputModel();
                CategoryViewModel viewModel = _performanceMonitorWorkerService.Category(catInputModel);
                return View("Category", viewModel);
            }

            string categoryName = inputModel.CategoryName;
            bool multiInstanceType = _performanceMonitorWorkerService.CheckForMultiInstanceType(categoryName);
            if (multiInstanceType)
            {
                return RedirectToAction("CategoryInstance", "PerformanceMonitor", new { selectedCategoryName = categoryName });
            }
            return RedirectToAction("Edit", "PerformanceMonitor", new { selectedCategoryName = categoryName, selectedInstanceName = "none" });
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult CategoryInstance(string selectedCategoryName)
        {
            CategoryInstanceInputModel inputModel = new CategoryInstanceInputModel();
            inputModel.CategoryName = selectedCategoryName;
            CategoryInstanceViewModel viewModel = _performanceMonitorWorkerService.CategoryInstance(inputModel);
            viewModel.CategoryName = selectedCategoryName;
            if (viewModel.Accessible == false)
            {
                return RedirectToAction("Category");
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("CategoryInstance")]
        //[HandleError, OrchardAuthorization(PermissionName = ApplicationFrameworkPermissionStrings.EditConfigurationSettings)]
        public ActionResult CategoryInstancePost(CategoryInstancePostInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                CategoryInstanceInputModel catInstanceInputModel = new CategoryInstanceInputModel();
                catInstanceInputModel.CategoryName = inputModel.CategoryName;
                CategoryInstanceViewModel viewModel = _performanceMonitorWorkerService.CategoryInstance(catInstanceInputModel);
                return View("CategoryInstance", viewModel);
            }

            string categoryName = inputModel.CategoryName;
            string instanceName = inputModel.InstanceName;
            return RedirectToAction("Edit", "PerformanceMonitor", new { selectedCategoryName = categoryName, selectedInstanceName = instanceName });
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = ApplicationFrameworkPermissionStrings.EditConfigurationSettings)]
        public ActionResult Edit(string selectedCategoryName, string selectedInstanceName)
        {
            EditInputModel inputModel = new EditInputModel();

            inputModel.CategoryName = selectedCategoryName;
            inputModel.InstanceName = selectedInstanceName.ToString();
            EditViewModel viewModel = _performanceMonitorWorkerService.Edit(inputModel);
            viewModel.InstanceName = selectedInstanceName.ToString();

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        //[HandleError, OrchardAuthorization(PermissionName = ApplicationFrameworkPermissionStrings.EditConfigurationSettings)]
        public ActionResult EditPost(EditPostInputModel inputModel)
        {
            EditInputModel newEditInputModel = new EditInputModel()
            {
                Id = inputModel.Id,
                CategoryName = inputModel.CategoryName,
                InstanceName = inputModel.InstanceName,
                CounterName = inputModel.CounterName,
                Duration = inputModel.Duration,
                SampleInterval = inputModel.SampleInterval
            };

            if (!ModelState.IsValid)
            {
                EditViewModel viewModel = _performanceMonitorWorkerService.Edit(newEditInputModel);
                return View("Edit", viewModel);
            }

            _performanceMonitorWorkerService.EditPost(inputModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult Show(ShowInputModel inputModel)
        {
            int performanceMonitorRecordId = inputModel.PerformanceMonitorRecord_Id;

            ShowViewModel viewModel = _performanceMonitorWorkerService.Show(performanceMonitorRecordId);
            viewModel.PerformanceMonitorRecord_Id = performanceMonitorRecordId;

            if (inputModel.AutoRefresh == true)
            {
                viewModel.AutoRefresh = true;
            }
            else
            {
                viewModel.AutoRefresh = false;
            }

            PerformanceMonitorRecord currentMonitorRecord = _performanceMonitorWorkerService.GetPerformanceMonitorRecord(performanceMonitorRecordId);

            int maxLength = 20;
            string labelCounter = string.Empty;
            var counterNameString = currentMonitorRecord.CounterName;
            if (counterNameString.Length <= maxLength)
            {
                labelCounter = counterNameString;
            }
            else
            {
                labelCounter = Truncate(counterNameString, maxLength);
                labelCounter = string.Concat(labelCounter, "...");
            }
            viewModel.LabelCounter = labelCounter;
            viewModel.Threshold = currentMonitorRecord.Threshold;
            viewModel.ThresholdWhen = currentMonitorRecord.ThresholdWhen;
            viewModel.SampleInterval = currentMonitorRecord.SampleInterval;

            viewModel.MeanValue = currentMonitorRecord.Mean;
            viewModel.MinimumValue = currentMonitorRecord.Minimum;
            viewModel.MaximumValue = currentMonitorRecord.Maximum;

            List<PerformanceMonitorDataRecord> dataRecordsToShow = _performanceMonitorWorkerService.Plot(performanceMonitorRecordId);
            viewModel.LineName = "Countervalues";
            viewModel.LabelAxisY = currentMonitorRecord.CounterName.ToString();
            viewModel.id = 1;
            viewModel.data = new List<PlotDataViewModel>();

            foreach (PerformanceMonitorDataRecord dataRecord in dataRecordsToShow)
            {
                double valueCount = double.Parse(dataRecord.Count);
                viewModel.data.Add(new PlotDataViewModel()
                {
                    Ticks = dataRecord.Ticks,
                    Value = valueCount
                });
            }
            return View(viewModel);
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult SetAutorefreshValue(int id, bool autorefresh)
        {
            ShowInputModel inputModel = new ShowInputModel();
            inputModel.PerformanceMonitorRecord_Id = id;
            inputModel.AutoRefresh = autorefresh;
            return RedirectToAction("Show", inputModel);
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult Delete(int id)
        {
            _performanceMonitorWorkerService.DeletePermanceMonitorRecord(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        //[HandleError, OrchardAuthorization(PermissionName = OrchardPermissionStrings.SiteOwner)]
        public ActionResult StopCounter(int performanceMonitorRecordId)
        {
            _performanceMonitorWorkerService.StopCounter(performanceMonitorRecordId);
            return RedirectToAction("Index");
        }

    }
}