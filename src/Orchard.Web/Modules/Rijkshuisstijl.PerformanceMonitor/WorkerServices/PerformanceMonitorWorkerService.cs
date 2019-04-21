#region Usings

using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.UI.Notify;
//using Rijkshuisstijl.ApplicationFramework.Services;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.DataServices;
using Rijkshuisstijl.PerformanceMonitor.Services;
using Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor;
using Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor;
using System.Web.Mvc.Async;


#endregion

namespace Rijkshuisstijl.PerformanceMonitor.WorkerServices
{
    public class PerformanceMonitorWorkerService : IPerformanceMonitorWorkerService
    {
        private readonly INotifier _notifier;
        private readonly IPerformanceMonitorService _performanceMonitorService;
        private readonly IPerformanceMonitorDataService _performanceMonitorDataService;
        private readonly IPerformanceCounterService _performanceCounterService;

        public PerformanceMonitorWorkerService(
                IPerformanceMonitorService performanceMonitorService,
                IPerformanceMonitorDataService performanceMonitorDataService,
                IPerformanceCounterService performanceCounterService,
                INotifier notifier
            )
        {
            _performanceMonitorService = performanceMonitorService;
            _performanceMonitorDataService = performanceMonitorDataService;
            _performanceCounterService = performanceCounterService;
            _notifier = notifier;
        }

        public Localizer T { get; set; }

        public IndexViewModel Index()
        {
            return new IndexViewModel 
            { 
                PerformanceMonitorRecords = _performanceMonitorDataService.PerformanceMonitorRecords
            };
        }

        public CategoryViewModel Category(CategoryInputModel inputModel)
        {
            CategoryViewModel viewModel = new CategoryViewModel();

            //initialize the dropdownlist values
            string ex;
            var performanceCounterCategories = _performanceMonitorService.CategoryNames(out ex).ToList();

            if (ex == string.Empty)
            {
                var categoryList = performanceCounterCategories.Select(c => new SelectListItem
                {
                    Text = c.ToString(),
                    Value = c.ToString()
                });

                viewModel.Accessible = true;
                viewModel.CategoryList = new SelectList(categoryList, "Value", "Text", inputModel.CategoryName);
                return viewModel;
            }
            else
            {
                _notifier.Error(T("Error: Not able to load the categories. Exception: {0} ", ex));
                viewModel.Accessible = false;
                return viewModel;
            }
        }

        public CategoryInstanceViewModel CategoryInstance(CategoryInstanceInputModel inputModel)
        {
            //initialize the dropdownlist values
            string myCatName = inputModel.CategoryName;
            var performanceCounterInstanceNames = _performanceMonitorService.GetInstanceNames(myCatName);

            var instanceList = performanceCounterInstanceNames.Select(c => new SelectListItem
            {
                Text = c.ToString(),
                Value = c.ToString()
            });

            bool accessibleValue = true;
            if (performanceCounterInstanceNames.Count == 0)
            {
                _notifier.Error(T("Error: Category and instances not accessible"));
                accessibleValue = false;
            }
            CategoryInstanceViewModel viewModel = new CategoryInstanceViewModel()
            {
                InstanceList = new SelectList(instanceList, "Value", "Text", inputModel.InstanceName),
                Accessible = accessibleValue
            };

            return viewModel;
        }

        public EditViewModel Edit(EditInputModel inputModel)
        {
            PerformanceMonitorRecord recordToEdit = _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.Id == inputModel.Id);
            if (recordToEdit != null)
            {
                //edit an existing record (no implementation in current version)
            }

            //No existing record in the database with the given id, create a new one
            if (recordToEdit == null)
            {
                recordToEdit = new PerformanceMonitorRecord();

                recordToEdit.Id = 0;
                recordToEdit.CategoryName = inputModel.CategoryName;
                if (inputModel.InstanceName != null)
                {
                    recordToEdit.InstanceName = inputModel.InstanceName;
                }

                recordToEdit.CounterName = inputModel.CounterName;

                if (inputModel.SampleInterval <= 0)
                {
                    recordToEdit.SampleInterval = 1;
                }
                else
                {
                    recordToEdit.SampleInterval = inputModel.SampleInterval;
                }

                if (inputModel.Duration == null || Int32.Parse(inputModel.Duration) <= 0)
                {
                    recordToEdit.Duration = "1";
                }
                else
                {
                    recordToEdit.Duration = inputModel.Duration;
                }
                recordToEdit.Threshold = 0;
                //ThresholdWhen default value is 'Above'
                recordToEdit.ThresholdWhen = false;
            }

            string myCatName = inputModel.CategoryName;
            List<string> performanceCounters;
            if (inputModel.InstanceName == "none")
            {
                performanceCounters = _performanceMonitorService.GetCounterNames(myCatName, null);
            }
            else
            {
                string myInstanceName = inputModel.InstanceName;
                performanceCounters = _performanceMonitorService.GetCounterNames(myCatName,myInstanceName);
            }

            var counterList = performanceCounters.Select(c => new SelectListItem
            {
                Text = c.ToString(),
                Value = c.ToString()
            });


            Dictionary<string, string> thresholdWhenValues = new Dictionary<string, string>();
            thresholdWhenValues.Add("false", "Above");
            thresholdWhenValues.Add("true", "Below");

            var thresholdWhenList = thresholdWhenValues.Select(c => new SelectListItem
            {
                Value = c.Key,
                Text = c.Value
            });

            EditViewModel viewModel = new EditViewModel()
            {
                Id = recordToEdit.Id,
                CategoryName = inputModel.CategoryName,
                CounterList = new SelectList(counterList, "Value", "Text", recordToEdit.CounterName),
                ThresholdWhenList = new SelectList(thresholdWhenList, "Value", "Text",recordToEdit.ThresholdWhen),
                Duration = recordToEdit.Duration,
                SampleInterval = recordToEdit.SampleInterval
            };

            return viewModel;
        }

        public bool EditPost(EditPostInputModel inputModel)
        {
            string performanceCounterReading =
                _performanceCounterService.GetPerformanceCounterReading(inputModel.CategoryName,inputModel.InstanceName,inputModel.CounterName);

            if (performanceCounterReading == String.Empty)
            {
                _notifier.Error(T("Error: Not able to perform a reading on this type of counter"));
                return false;
            }

            PerformanceMonitorRecord record = new PerformanceMonitorRecord()
            {
                Id = inputModel.Id,
                CategoryName = inputModel.CategoryName,
                InstanceName = inputModel.InstanceName,
                CounterName = inputModel.CounterName,
                InitialValue = performanceCounterReading,
                Duration = inputModel.Duration,
                SampleInterval = inputModel.SampleInterval,
                Threshold = inputModel.Threshold,
                ThresholdWhen = inputModel.ThresholdWhen
            };

            var result = _performanceMonitorDataService.SetPerformanceMonitorRecord(record);
            if (result)
            {
                PerformanceMonitorRecord activeRecord =
                    _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.IsEnabled == true);

                //threshold check
                bool passedThreshold;
                var passedThresholdValue = 0;
                double countervalue = double.Parse(activeRecord.InitialValue);

                passedThreshold = _performanceMonitorService.CheckThreshold(activeRecord.Threshold, activeRecord.ThresholdWhen, countervalue, ref passedThresholdValue);

                PerformanceMonitorDataRecord datarecord = new PerformanceMonitorDataRecord() 
                {
                    PerformanceMonitorRecord_Id = activeRecord.Id,
                    Count = activeRecord.InitialValue,
                    HeartBeat = activeRecord.Created,
                    PassedThreshold = passedThreshold,
                    PassedThresholdValue = passedThresholdValue,

                    //duration count(ticks in nanoseconds from starttime (created)) should initially be set to startvalue (zero)
                    Ticks = "0"
                };

                var dataResult = _performanceMonitorDataService.WriteDataRecord(datarecord);
                if (dataResult)
                {
                    _notifier.Information(T("New counter records added succesfully"));
                }
                else
                {
                    _notifier.Warning(T("New counter record added succesfully, but failed to write initial datarecord"));
                }
            }
            else
            {
                _notifier.Error(T("Error: Failed to add new counter"));
            }
            return result;
        }

        public ShowViewModel Show(int performanceMonitorRecordId)
        {
            List<PerformanceMonitorDataRecord> dataRecordsToShow =
                _performanceMonitorDataService.GetDataRecords(performanceMonitorRecordId);

            return new ShowViewModel { PerformanceMonitorDataRecords = dataRecordsToShow };
        }

        public List<PerformanceMonitorDataRecord> Plot(int id)
        {
            List<PerformanceMonitorDataRecord> dataRecordsToShow =
                _performanceMonitorDataService.GetDataRecords(id);

            return dataRecordsToShow;
        }

        public PerformanceMonitorRecord DeletePermanceMonitorRecord(int id) 
        {
            PerformanceMonitorRecord recordToDelete =
                _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.Id == id);

            if (recordToDelete == null)
            {
                _notifier.Error(T("Could not find the performance counter record with id {0}", id));
                return null;
            }
           
            //check and delete childrecords
            List<PerformanceMonitorDataRecord> dataRecordsToDelete =
                _performanceMonitorDataService.GetDataRecords(id);

            if (dataRecordsToDelete.Count > 0)
            {
                foreach(PerformanceMonitorDataRecord dataRecordToDelete in dataRecordsToDelete)
                {
                    _performanceMonitorDataService.DeletePerformanceMonitorDataRecord(dataRecordToDelete);
                }             
            }

            if (_performanceMonitorDataService.DeletePerformanceMonitorRecord(recordToDelete))
            {

                _notifier.Information(T("performance counter is deleted"));
            }
            else
            {
                _notifier.Error(T("Failed to delete performance counter"));
            }

            return recordToDelete;
        }

        public bool StopCounter(int id)
        {
            PerformanceMonitorRecord recordToUpdate =
                _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.Id == id);

            string performanceCounterReading =
                    _performanceCounterService.GetPerformanceCounterReading(recordToUpdate.CategoryName, recordToUpdate.InstanceName, recordToUpdate.CounterName);

            //threshold check
            bool passedThreshold;
            var passedThresholdValue = 0;
            double countervalue = double.Parse(performanceCounterReading);
            passedThreshold = _performanceMonitorService.CheckThreshold(recordToUpdate.Threshold, recordToUpdate.ThresholdWhen, countervalue, ref passedThresholdValue);

            //calculate the the duration (in ticks)
            DateTime startTime = (DateTime)recordToUpdate.Created;
            TimeSpan durationCount = DateTime.Now.Subtract(startTime);

            DateTime sharedTimeStampValue = DateTime.Now;

            //first write last values to datarecord (then to the monitorrecord for min/max/average)
            PerformanceMonitorDataRecord datarecord = new PerformanceMonitorDataRecord()
            {
                PerformanceMonitorRecord_Id = recordToUpdate.Id,
                //should be finally equal to latest reading in performancemonitorrecord
                Count = performanceCounterReading,
                HeartBeat = sharedTimeStampValue,
                PassedThreshold = passedThreshold,
                PassedThresholdValue = passedThresholdValue,
                //duration count(ticks in nanoseconds from starttime (created)) should be equal to value in performancemonitorrecord
                Ticks = Convert.ToString(durationCount.Ticks)
            };

            var result = _performanceMonitorDataService.WriteDataRecord(datarecord);
            if (result)
            {
                recordToUpdate.LastValue = performanceCounterReading;
                recordToUpdate.DurationCount = Convert.ToString(durationCount.Ticks);
                recordToUpdate.EndTime = sharedTimeStampValue;

                //mean, min and max values
                List<PerformanceMonitorDataRecord> dataRecords = _performanceMonitorDataService.GetDataRecords(id);
                //first convert counted values from string to int
                List<double> countedValues = new List<double>();
                bool conversionResult = false;
                foreach (PerformanceMonitorDataRecord record in dataRecords)
                {
                    double value;
                    bool conversionPassed = double.TryParse(record.Count, out value);
                    if (conversionPassed)
                    {
                        countedValues.Add(value);
                        conversionResult = true;
                    }
                    else
                    {
                        //conversion failed
                        conversionResult = false;
                    }
                }
                double meanValue = 0;
                double minValue = 0;
                double maxValue = 0;

                if (conversionResult)
                {
                    meanValue = Math.Round(countedValues.Average(), 2);
                    minValue = Math.Round(countedValues.Min(), 2);
                    maxValue = Math.Round(countedValues.Max(), 2);
                }
                recordToUpdate.Mean = meanValue; 
                recordToUpdate.Minimum = minValue;
                recordToUpdate.Maximum = maxValue;

                var monitorrecordresult = _performanceMonitorDataService.UpdatePerformanceMonitorRecord(recordToUpdate);
                if (monitorrecordresult)
                {
                    _notifier.Information(T("Counter stopped and all values updated succesfully"));
                }
                else
                {
                    _notifier.Error(T("Error: Failed to stop counter and update the monitor record"));
                }
           }
            else
            {
                _notifier.Error(T("Error: Failed to stop counter and update all records"));
            }
            
            return result;
        }

        public bool CheckForMultiInstanceType(string categoryName)
        {
            bool isMultiInstance = _performanceMonitorService.GetInstanceType(categoryName);
            if (isMultiInstance)
            {
                return true;
            }

            return false;
        }

        public PerformanceMonitorRecord GetPerformanceMonitorRecord(int performanceMonitorRecordId)
        {
            PerformanceMonitorRecord record = _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.Id == performanceMonitorRecordId);
            return record;
        }

    }
}

