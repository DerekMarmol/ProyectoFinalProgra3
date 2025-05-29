// Models/DashboardViewModel.cs
namespace SuperBodega.AdminWeb.Models
{
    public class DashboardViewModel
    {
        public int CategoryCount { get; set; }
        public int ProductCount { get; set; }
        public int SupplierCount { get; set; }
        public decimal MonthlySales { get; set; }
        public List<RecentActivityItem> RecentActivities { get; set; } = new List<RecentActivityItem>();
    }

    public class RecentActivityItem
    {
        public string ActivityType { get; set; }  // "product", "category", "supplier", "sale"
        public string Action { get; set; }  // "added", "updated", "deleted"
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}