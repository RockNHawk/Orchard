using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Services;
using Rijkshuisstijl.PerformanceMonitor.Models;

namespace Rijkshuisstijl.PerformanceMonitor.DataServices
{
    public class PerformanceMonitorDataService : IPerformanceMonitorDataService
    {
        public const string PerformanceMonitorDataCache = "Rijkshuisstijl.PerformanceMonitor.Update";
        private readonly IClock _clock;
        private readonly IRepository<PerformanceMonitorRecord> _performanceMonitorRecordRepository;
        private readonly IRepository<PerformanceMonitorDataRecord> _performanceMonitorDataRecordRepository;
        private readonly ISignals _signals;
        private readonly WorkContext _workContext;

        public PerformanceMonitorDataService(
            IClock clock,
            ISignals signals,
            IRepository<PerformanceMonitorRecord> performanceMonitorRecordRepository,
            IRepository<PerformanceMonitorDataRecord> performanceMonitorDataRecordRepository,
            WorkContext workContext
            ) 
        { 
            _clock = clock;
            _signals = signals;
            _performanceMonitorRecordRepository = performanceMonitorRecordRepository;
            _performanceMonitorDataRecordRepository = performanceMonitorDataRecordRepository;
            _workContext = workContext;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public PerformanceMonitorRecord GetRecord(int id)
        {
            return PerformanceMonitorRecords.FirstOrDefault(r => r.Id == id);
        }

        public PerformanceMonitorRecord ActiveRecord
        {
            get
            {
                var record = PerformanceMonitorRecords.FirstOrDefault(r => r.IsEnabled == true);
                if (record == null)
                {
                    return record;
                }

                PerformanceMonitorRecord activeRecord = new PerformanceMonitorRecord
                {
                    Id = record.Id,
                    SampleInterval = record.SampleInterval,
                    EndTime = record.EndTime
                };
                return activeRecord;
            }
        }

        public List<PerformanceMonitorRecord> PerformanceMonitorRecords
        {
            get 
            { 
                List<PerformanceMonitorRecord> performanceMonitorRecords;
                performanceMonitorRecords = _performanceMonitorRecordRepository.Table.ToList();

                return performanceMonitorRecords;
            }
        }

        //initial
        public bool SetPerformanceMonitorRecord(PerformanceMonitorRecord performanceMonitorRecord)
        {
            if (performanceMonitorRecord == null)
            {
                return false;
            }

            string userName = _workContext.CurrentUser.UserName;
            performanceMonitorRecord.CreatedBy = userName;
            performanceMonitorRecord.Created = DateTime.Now;
            performanceMonitorRecord.StartTime = DateTime.Now;
            performanceMonitorRecord.NodeName = Environment.MachineName.ToUpperInvariant();

            IPAddress localIpAdress = IPAddress.Parse("127.0.0.1");
            performanceMonitorRecord.IpAddress = localIpAdress.ToString();
            performanceMonitorRecord.IsEnabled = true;

            performanceMonitorRecord.EndTime = DateTime.Now.AddHours(Convert.ToDouble(performanceMonitorRecord.Duration));

            //performanceMonitorRecord from string "1-24" to  double value to timespan ticks (nanoseconds)
            TimeSpan durationCount = TimeSpan.FromHours(Convert.ToDouble(performanceMonitorRecord.Duration));
            performanceMonitorRecord.Duration = durationCount.Ticks.ToString();

            _performanceMonitorRecordRepository.Update(performanceMonitorRecord);

            return true;
        }

        //counter stopped : update records
        public bool UpdatePerformanceMonitorRecord(PerformanceMonitorRecord performanceMonitorRecord)
        {
            if (performanceMonitorRecord == null)
            {
                return false;
            }

            performanceMonitorRecord.IsEnabled = false;
            _performanceMonitorRecordRepository.Update(performanceMonitorRecord);

            return true;
        }

        public bool DeletePerformanceMonitorRecord(PerformanceMonitorRecord recordToDelete)
        {
            if (recordToDelete == null)
            {
                return false;
            }
            _performanceMonitorRecordRepository.Delete(recordToDelete);

            return true;
        }

        public bool WriteDataRecord(PerformanceMonitorDataRecord performanceMonitorDataRecord)
        {
            _performanceMonitorDataRecordRepository.Create(performanceMonitorDataRecord);

            return true;
        }

        public List<PerformanceMonitorDataRecord> GetDataRecords(int id)
        {
            return PerformanceMonitorDataRecords.Where(r => r.PerformanceMonitorRecord_Id == id).ToList();
        }

        public List<PerformanceMonitorDataRecord> PerformanceMonitorDataRecords
        {
            get
            {
                List<PerformanceMonitorDataRecord> performanceMonitorDataRecords;
                performanceMonitorDataRecords = _performanceMonitorDataRecordRepository.Table.OrderBy(r => r.Id).ToList();

                return performanceMonitorDataRecords;
            }
        }

        public bool DeletePerformanceMonitorDataRecord(PerformanceMonitorDataRecord dataRecordToDelete)
        {
            if (dataRecordToDelete == null)
            {
                return false;
            }
            _performanceMonitorDataRecordRepository.Delete(dataRecordToDelete);

            return true;
        }
    }
}