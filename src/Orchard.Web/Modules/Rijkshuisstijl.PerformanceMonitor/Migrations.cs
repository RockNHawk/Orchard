#region

using System;
using Orchard.Data;
using Orchard.Data.Migration;
using Rijkshuisstijl.PerformanceMonitor.Models;

#endregion

namespace Rijkshuisstijl.PerformanceMonitor
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            //Create the tables 
            SchemaBuilder.CreateTable(typeof(PerformanceMonitorRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("CategoryName")
                .Column<string>("InstanceName", c => c.Nullable())
                .Column<string>("CounterName")
                .Column<string>("NodeName")
                .Column<string>("IpAddress")
                .Column<string>("Duration")
                .Column<int>("SampleInterval")
                .Column<int>("Threshold")
                .Column<bool>("ThresholdWhen")
                .Column<DateTime>("Created")
                .Column<string>("CreatedBy")
                .Column<bool>("IsEnabled")
                .Column<string>("InitialValue")
                .Column<string>("LastValue")
                .Column<DateTime>("StartTime")
                .Column<DateTime>("EndTime")
                .Column<string>("DurationCount")
                .Column<double>("Mean")
                .Column<double>("Minimum")
                .Column<double>("Maximum"))
                .AlterTable(typeof (PerformanceMonitorRecord).Name, table => table
                    .CreateIndex("IDX_ID", new[] {"Id"}));

            SchemaBuilder.CreateTable(typeof(PerformanceMonitorDataRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("PerformanceMonitorRecord_Id")
                .Column<string>("Count")
                .Column<DateTime>("HeartBeat")
                .Column<string>("Ticks")
                .Column<bool>("PassedThreshold")
                .Column<int>("PassedThresholdValue")
                );

            SchemaBuilder.CreateForeignKey(
                    "FK_PerformanceMonitorDataRecord_PerformanceMonitorRecord"
                    , "PerformanceMonitorDataRecord", new[] { "PerformanceMonitorRecord_Id" }
                    , "PerformanceMonitorRecord", new[] { "Id" }
                );

            return 1;
        }

    }
}