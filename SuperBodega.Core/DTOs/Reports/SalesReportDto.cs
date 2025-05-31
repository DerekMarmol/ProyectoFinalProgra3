using System;
using System.Collections.Generic;

namespace SuperBodega.Core.DTOs.Reports
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
        public List<ProductSalesDto> TopProducts { get; set; } = new List<ProductSalesDto>();
        public List<CategorySalesDto> SalesByCategory { get; set; } = new List<CategorySalesDto>();
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int OrderCount { get; set; }
    }

    public class ProductSalesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategorySalesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalSales { get; set; }
        public int ProductCount { get; set; }
        public decimal Percentage { get; set; }
    }
}