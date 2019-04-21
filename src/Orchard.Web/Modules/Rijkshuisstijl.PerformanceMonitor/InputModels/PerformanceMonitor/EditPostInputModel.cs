using System;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class EditPostInputModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string InstanceName { get; set; }

        [Required(ErrorMessage = "Countername must be selected")]
        public string CounterName { get; set; }

        public bool IsEnabled { get; set; }

        [Range(0, 24, ErrorMessage = "Duration can only be set to a maximum of 24 hours")]
        public string Duration { get; set; }

        [Range(1, 60, ErrorMessage = "Interval can only be set to a minimum of 1 and a maximum of 60 minutes")]
        public int SampleInterval { get; set; }

        public int Threshold { get; set; }
        public bool ThresholdWhen { get; set; }

        public string LastValue { get; set; }
        public string InitialValue { get; set; }
        public SelectList CounterList { get; set; }
    }
}