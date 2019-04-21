using System;
using System.Web.Mvc;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.Models;

namespace Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor
{
    public class EditViewModel
    {
        public EditViewModel()
        {
        }

        public EditViewModel(EditPostInputModel inputModel)
        {
            Id = inputModel.Id;
            CategoryName = inputModel.CategoryName;
            InstanceName = inputModel.InstanceName;
            CounterName = inputModel.CounterName;
            Duration = inputModel.Duration;
            SampleInterval = inputModel.SampleInterval;
            Threshold = inputModel.Threshold;
            ThresholdWhen = inputModel.ThresholdWhen;
        }

        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string CategoryName { get; set; }
        public string InstanceName { get; set; }
        public string CounterName { get; set; }
        public string Duration { get; set; }
        public int SampleInterval { get; set; }
        public int Threshold { get; set; }
        public bool ThresholdWhen { get; set; }
        public string LastValue { get; set; }
        public string InitialValue { get; set; }

        public SelectList CategoryList { get; set; }
        public SelectList CounterList { get; set; }

        public SelectList ThresholdWhenList { get; set; }
    }
}