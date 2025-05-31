using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperBodega.AdminWeb.Models.Reports
{
    public class SalesReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesViewModel> DailySales { get; set; } = new List<DailySalesViewModel>();
        public List<ProductSalesViewModel> TopProducts { get; set; } = new List<ProductSalesViewModel>();
        public List<CategorySalesViewModel> SalesByCategory { get; set; } = new List<CategorySalesViewModel>();
    }

    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int OrderCount { get; set; }
    }

    public class ProductSalesViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategorySalesViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalSales { get; set; }
        public int ProductCount { get; set; }
        public decimal Percentage { get; set; }
    }
}