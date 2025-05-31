namespace SuperBodega.AdminWeb.Models.Reports
{
    public class DashboardSummaryViewModel
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