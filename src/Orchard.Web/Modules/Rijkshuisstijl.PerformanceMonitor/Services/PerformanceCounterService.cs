using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Orchard;
using Orchard.Caching;

namespace Rijkshuisstijl.PerformanceMonitor.Services
{
    public class PerformanceCounterService : IPerformanceCounterService
    {
        public string GetPerformanceCounterReading(string categoryName, string instanceName, string counterName)
        {
            string performanceCounterReading = null;

            PerformanceCounter counter = null;
            if (instanceName == "none")
            {
                counter = new PerformanceCounter(categoryName, counterName);
            }
            else
            {
                counter = new PerformanceCounter(categoryName, counterName, instanceName);
            }

            try
            {
                performanceCounterReading = counter.NextValue().ToString();

                //Thread has to sleep for at least 1 sec for accurate value.
                System.Threading.Thread.Sleep(1000);

                performanceCounterReading = counter.NextValue().ToString();
            }
            catch
            {
                performanceCounterReading = String.Empty;
                return performanceCounterReading;
            }

            return performanceCounterReading;
        }
    }
}