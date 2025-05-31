using System.Collections.Generic;

namespace SuperBodega.Core.DTOs.Reports
{
    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public List<LowStockProductDto> LowStockItems { get; set; } = new List<LowStockProductDto>();
        public List<TopInventoryValueDto> TopValueProducts { get; set; } = new List<TopInventoryValueDto>();
        public List<CategoryInventoryDto> InventoryByCategory { get; set; } = new List<CategoryInventoryDto>();
    }

    public class LowStockProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; } = 5; // Configurable
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
    }

    public class TopInventoryValueDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryInventoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
    }
}