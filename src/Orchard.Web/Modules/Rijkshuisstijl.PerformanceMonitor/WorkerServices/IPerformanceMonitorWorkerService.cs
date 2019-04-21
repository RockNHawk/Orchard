using Orchard;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor;
using System.Collections.Generic;

namespace Rijkshuisstijl.PerformanceMonitor.WorkerServices
{
    public interface IPerformanceMonitorWorkerService : IDependency
    {
        IndexViewModel Index();
        CategoryViewModel Category(CategoryInputModel inputModel);
        CategoryInstanceViewModel CategoryInstance(CategoryInstanceInputModel inputModel);
        EditViewModel Edit(EditInputModel inputModel);
        bool EditPost(EditPostInputModel inputModel);
        ShowViewModel Show(int performanceMonitorRecordId);
        List<PerformanceMonitorDataRecord> Plot(int id);
        PerformanceMonitorRecord DeletePermanceMonitorRecord(int id);
        bool StopCounter(int id);
        bool CheckForMultiInstanceType(string categoryName);
        PerformanceMonitorRecord GetPerformanceMonitorRecord(int performanceMonitorRecordId);
    } 
}