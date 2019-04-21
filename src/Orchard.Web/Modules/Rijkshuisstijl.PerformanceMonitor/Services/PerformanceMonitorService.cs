using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Orchard;
using Orchard.Caching;

using Rijkshuisstijl.PerformanceMonitor.DataServices;
using Rijkshuisstijl.PerformanceMonitor.Models;

namespace Rijkshuisstijl.PerformanceMonitor.Services
{
    public class PerformanceMonitorService : IPerformanceMonitorService
    {
        private readonly IPerformanceMonitorDataService _performanceMonitorDataService;
        private readonly IPerformanceCounterService _performanceCounterService;

        public PerformanceMonitorService(IPerformanceMonitorDataService performanceMonitorDataService, IPerformanceCounterService performanceCounterService)
        {
            _performanceMonitorDataService = performanceMonitorDataService;
            _performanceCounterService = performanceCounterService;
        }

        public List<string> CategoryNames(out string ex)
        {
            ex = string.Empty;

            PerformanceCounterCategory[] myCategories = null;
            List<string> performanceCounterCategories = new List<string>();

            string node = Environment.MachineName.ToUpperInvariant();
            try
            {
                //get all the categories on the system
                myCategories = PerformanceCounterCategory.GetCategories(node);
                foreach (var performanceCounterCategory in myCategories)
                {
                    performanceCounterCategories.Add(performanceCounterCategory.CategoryName.ToString());
                }

                List<string> categoriesInOrder = new List<string>() { };
                categoriesInOrder = performanceCounterCategories.OrderBy(c => c).ToList();

                return categoriesInOrder;
            }
            catch (Exception e)
            {
                ex = e.ToString();
                return performanceCounterCategories;
            }
        }

        public bool GetInstanceType(string categoryName)
        {
            PerformanceCounterCategory[] myCategories =
                    PerformanceCounterCategory.GetCategories();

            IEnumerable<PerformanceCounterCategory> myCat = myCategories.Where(r => r.CategoryName == categoryName);
            PerformanceCounterCategoryType categoryType = myCat.FirstOrDefault().CategoryType;

            if (categoryType == PerformanceCounterCategoryType.MultiInstance)
            {
                return true;
            }
            return false;
        }

        public List<string> GetInstanceNames(string categoryName)
        {
            System.Diagnostics.PerformanceCounterCategory[] myCategories;
            List<string> performanceCounterInstanceNames = new List<string>();

            myCategories =
                System.Diagnostics.PerformanceCounterCategory.GetCategories();

            foreach (var performanceCounterCategory in myCategories.Where(r => r.CategoryName == categoryName))
            {
                var instanceNames = performanceCounterCategory.GetInstanceNames();
                foreach (var instanceName in instanceNames)
                {
                    performanceCounterInstanceNames.Add(instanceName.ToString());
                }

            }

            List<string> instanceNamesInOrder = new List<string>() { };
            instanceNamesInOrder = performanceCounterInstanceNames.OrderBy(c => c).ToList();

            return instanceNamesInOrder;
        }

        public List<string> GetCounterNames(string categoryName, string instanceName)
        {
            PerformanceCounterCategory[] myCategories = PerformanceCounterCategory.GetCategories();
            List<string> performanceCounterNames = new List<string>();

            foreach (var performanceCounterCategory in myCategories.Where(r => r.CategoryName == categoryName))
            {
                PerformanceCounter[] counters = null;
                if (instanceName == null)
                {
                    counters = performanceCounterCategory.GetCounters();
                }
                else
                {
                    counters = performanceCounterCategory.GetCounters(instanceName.ToString());
                }

                foreach (var performanceCounter in counters)
                {
                    performanceCounterNames.Add(performanceCounter.CounterName.ToString());
                }
            }

            List<string> countersInOrder = new List<string>() { };
            countersInOrder = performanceCounterNames.OrderBy(c => c).ToList();

            return countersInOrder;
        }

        public void HeartBeat()
        {
            //perform and process the counterreading every 'heartbeat'
            PerformanceMonitorRecord activeMonitorRecord =
                _performanceMonitorDataService.PerformanceMonitorRecords.FirstOrDefault(r => r.IsEnabled == true);

            string performanceCounterReading =
                    _performanceCounterService.GetPerformanceCounterReading(activeMonitorRecord.CategoryName, activeMonitorRecord.InstanceName, activeMonitorRecord.CounterName);

            //calculate the the duration (in ticks)
            DateTime startTime = (DateTime)activeMonitorRecord.Created;
            TimeSpan durationCount = DateTime.Now.Subtract(startTime);

            //threshold check
            bool passedThreshold;
            var passedThresholdValue = 0;
            double countervalue = double.Parse(performanceCounterReading);

            passedThreshold = CheckThreshold(activeMonitorRecord.Threshold, activeMonitorRecord.ThresholdWhen, countervalue, ref passedThresholdValue);

            PerformanceMonitorDataRecord dataRecord = new PerformanceMonitorDataRecord()
            {
                PerformanceMonitorRecord_Id = activeMonitorRecord.Id,
                Count = performanceCounterReading,
                HeartBeat = DateTime.Now,
                PassedThreshold = passedThreshold,
                PassedThresholdValue = passedThresholdValue,
                Ticks = Convert.ToString(durationCount.Ticks)
            };

            _performanceMonitorDataService.WriteDataRecord(dataRecord);
        }

        public bool CheckThreshold(int thresholdValue, bool thresholdWhen, double countervalue,ref int passedThresholdValue)
        {
            bool passedThreshold = false;

            if (thresholdValue == 0)
            {
            }
            else
            {
                if (!thresholdWhen)
                {
                    //above
                    if (thresholdValue < countervalue)
                    {
                        var diff = Math.Abs(countervalue - thresholdValue);
                        // need some min threshold to compare floating points
                        if (diff < 0.0000001)
                        {
                        }
                        else
                        {
                            passedThreshold = true;
                            passedThresholdValue = Convert.ToInt32(diff);
                        }
                    }
                }

                if (thresholdWhen)
                {
                    //below
                    if (countervalue < thresholdValue)
                    {
                        var diff = Math.Abs(thresholdValue - countervalue);
                        // need some min threshold to compare floating points
                        if (diff < 0.0000001)
                        {
                        }
                        else
                        {
                            passedThreshold = true;
                            passedThresholdValue = Convert.ToInt32(diff);
                        }
                    }
                }
            }
            return passedThreshold;
        }
    }
}