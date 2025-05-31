using System.Threading.Tasks;
using SuperBodega.Core.DTOs.Reports;

namespace SuperBodega.Core.Services
{
    public interface IReportService
    {
        Task<SalesReportDto> GetSalesReportAsync(ReportFilterDto filter);
        Task<InventoryReportDto> GetInventoryReportAsync();
        Task<CustomersReportDto> GetCustomersReportAsync(ReportFilterDto filter);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    }

    public class DashboardSummaryDto
    {
        public decimal TodaySales { get; set; }
        public decimal MonthSales { get; set; }
        public decimal YearSales { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalCustomers { get; set; }
        public decimal GrowthPercentage { get; set; }
    }
}