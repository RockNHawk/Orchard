using System;
using System.Web.Mvc;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.Models;

namespace Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor
{
    public class CategoryViewModel
    {
        public string CategoryName { get; set; }
        public SelectList CategoryList { get; set; }
        public bool Accessible { get; set; }
    }
}