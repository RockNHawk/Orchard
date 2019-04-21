using System;
using System.Web.Mvc;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.Models;

namespace Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor
{
    public class CategoryInstanceViewModel
    {
        public string CategoryName { get; set; }
        public string InstanceName { get; set; }
        public SelectList InstanceList { get; set; }
        public bool Accessible { get; set; }
    }
}