using Orchard;
using Orchard.Tasks;
using Orchard.UI.Notify;
using Rijkshuisstijl.PerformanceMonitor.DataServices;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.Services;
using Rijkshuisstijl.PerformanceMonitor.WorkerServices;
using System;
using System.Linq;

namespace Rijkshuisstijl.PerformanceMonitor.BackgroundTasks
{
    public class PerformanceMonitorHeartBeat : IBackgroundTask
    {
        private readonly IPerformanceMonitorWorkerService _performanceMonitorWorkerService;
        private readonly IPerformanceMonitorDataService _performanceMonitorDataService;
        private readonly IPerformanceMonitorService _performanceMonitorService;

        public PerformanceMonitorHeartBeat(IPerformanceMonitorWorkerService performanceMonitorWorkerService,IPerformanceMonitorService performanceMonitorService, IPerformanceMonitorDataService performanceMonitorDataService)
        {
            _performanceMonitorWorkerService = performanceMonitorWorkerService;
            _performanceMonitorService = performanceMonitorService;
            _performanceMonitorDataService = performanceMonitorDataService;
        }

        public void Sweep()
        {
            PerformanceMonitorDataRecord lastCounterDataRecord = _performanceMonitorDataService.PerformanceMonitorDataRecords.LastOrDefault();
            PerformanceMonitorRecord activeMonitorRecord = _performanceMonitorDataService.ActiveRecord;

            if (activeMonitorRecord == null)
            {
                return;
            }

            //calculate the the duration in seconds between the last registration and the current time of sweep()
            DateTime lastHeartBeat = lastCounterDataRecord.HeartBeat;
            TimeSpan totalDuration = DateTime.Now.Subtract(lastHeartBeat);
            double totalDurationSeconds = totalDuration.TotalSeconds;


            //stop the counter automatically if the endtime has been reached (or exceeded)
            if (activeMonitorRecord.EndTime <= DateTime.Now)
            {
                _performanceMonitorWorkerService.StopCounter(activeMonitorRecord.Id);
            }
            else
            {
                int interval = activeMonitorRecord.SampleInterval;
                double intervalSeconds = interval * 60;

                //write a new datarecord when the last heartbeat (datetime) was at least
                //the given interval (in minutes) in the past (interval negative)
                if (lastCounterDataRecord.HeartBeat < DateTime.Now.AddMinutes((interval * -1)))
                {
                    _performanceMonitorService.HeartBeat();
                }
                else
                {
                    //sometimes Sweep method runs outside of the minute interval
                    //to compensate, we use a little 'slack' 
                    //see: http://orchard.codeplex.com/workitem/20383
                    var diff = Math.Abs(intervalSeconds - totalDurationSeconds);
                    if (diff <= 10)
                    {
                        _performanceMonitorService.HeartBeat();
                    }
                }
            }
        }
    }
}