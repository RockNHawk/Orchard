using Orchard;
using Rijkshuisstijl.PerformanceMonitor.Models;
using System.Collections.Generic;

namespace Rijkshuisstijl.PerformanceMonitor.DataServices
{
    public interface IPerformanceMonitorDataService : IDependency
    {

        List<PerformanceMonitorRecord> PerformanceMonitorRecords { get; }
        List<PerformanceMonitorDataRecord> PerformanceMonitorDataRecords { get; }
        PerformanceMonitorRecord GetRecord(int id);
        List<PerformanceMonitorDataRecord> GetDataRecords(int id);
        PerformanceMonitorRecord ActiveRecord { get; }
        bool SetPerformanceMonitorRecord(PerformanceMonitorRecord record);
        bool UpdatePerformanceMonitorRecord(PerformanceMonitorRecord performanceMonitorRecord);
        bool DeletePerformanceMonitorRecord(PerformanceMonitorRecord recordToDelete);
        bool WriteDataRecord(PerformanceMonitorDataRecord performanceMonitorDataRecord);
        bool DeletePerformanceMonitorDataRecord(PerformanceMonitorDataRecord dataRecordToDelete);
    }
}