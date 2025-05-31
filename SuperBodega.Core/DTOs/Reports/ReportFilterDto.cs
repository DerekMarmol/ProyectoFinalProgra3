using System;

namespace SuperBodega.Core.DTOs.Reports
{
    public class ReportFilterDto
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public int? CustomerId { get; set; }
        public ReportPeriod Period { get; set; } = ReportPeriod.Last30Days;
    }

    public enum ReportPeriod
    {
        Today,
        Yesterday,
        Last7Days,
        Last30Days,
        LastMonth,
        Last3Months,
        Last6Months,
        LastYear,
        Custom
    }
}