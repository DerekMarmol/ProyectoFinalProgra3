using System.Collections.Generic;

namespace SuperBodega.AdminWeb.Models.Reports
{
    public class InventoryReportViewModel
    {
        public int TotalProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public List<LowStockProductViewModel> LowStockItems { get; set; } = new List<LowStockProductViewModel>();
        public List<TopInventoryValueViewModel> TopValueProducts { get; set; } = new List<TopInventoryValueViewModel>();
        public List<CategoryInventoryViewModel> InventoryByCategory { get; set; } = new List<CategoryInventoryViewModel>();
    }

    public class LowStockProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; } = 5;
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
    }

    public class TopInventoryValueViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryInventoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
    }
}
