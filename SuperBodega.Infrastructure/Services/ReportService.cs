using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperBodega.Core.DTOs.Reports;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;
using SuperBodega.Infrastructure.Data;

namespace SuperBodega.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalesReportDto> GetSalesReportAsync(ReportFilterDto filter)
        {
            var sales = await _context.Sales
                .Where(s => s.SaleDate >= filter.StartDate && 
                           s.SaleDate <= filter.EndDate && 
                           s.IsActive)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                        .ThenInclude(p => p.Category)
                .Include(s => s.Customer)
                .ToListAsync();

            var totalSales = sales.Sum(s => s.TotalAmount);
            var totalOrders = sales.Count;
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;

            // Ventas diarias
            var dailySales = sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new DailySalesDto
                {
                    Date = g.Key,
                    Amount = g.Sum(s => s.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Top productos - CORREGIDO: Evitar nombres duplicados en el tipo anónimo
            var topProducts = sales
                .SelectMany(s => s.SaleDetails)
                .GroupBy(sd => new { 
                    sd.ProductId, 
                    ProductName = sd.Product.Name, 
                    CategoryName = sd.Product.Category.Name 
                })
                .Select(g => new ProductSalesDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    QuantitySold = g.Sum(sd => sd.Quantity),
                    TotalRevenue = g.Sum(sd => sd.TotalPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(10)
                .ToList();

            // Ventas por categoría
            var salesByCategory = sales
                .SelectMany(s => s.SaleDetails)
                .GroupBy(sd => new { sd.Product.CategoryId, CategoryName = sd.Product.Category.Name })
                .Select(g => new CategorySalesDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalSales = g.Sum(sd => sd.TotalPrice),
                    ProductCount = g.Select(sd => sd.ProductId).Distinct().Count()
                })
                .ToList();

            // Calcular porcentajes
            foreach (var category in salesByCategory)
            {
                category.Percentage = totalSales > 0 ? (category.TotalSales / totalSales) * 100 : 0;
            }

            return new SalesReportDto
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue,
                DailySales = dailySales,
                TopProducts = topProducts,
                SalesByCategory = salesByCategory.OrderByDescending(c => c.TotalSales).ToList()
            };
        }

        public async Task<InventoryReportDto> GetInventoryReportAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();

            var totalProducts = products.Count;
            var totalInventoryValue = products.Sum(p => p.Stock * p.Price);
            var lowStockProducts = products.Count(p => p.Stock <= 5);
            var outOfStockProducts = products.Count(p => p.Stock == 0);

            // Productos con bajo stock
            var lowStockItems = products
                .Where(p => p.Stock <= 5 && p.Stock > 0)
                .Select(p => new LowStockProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    CurrentStock = p.Stock,
                    CategoryName = p.Category.Name,
                    SupplierName = p.Supplier.Name,
                    Price = p.Price
                })
                .OrderBy(p => p.CurrentStock)
                .ToList();

            // Productos con mayor valor en inventario
            var topValueProducts = products
                .Select(p => new TopInventoryValueDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Stock = p.Stock,
                    UnitPrice = p.Price,
                    TotalValue = p.Stock * p.Price,
                    CategoryName = p.Category.Name
                })
                .OrderByDescending(p => p.TotalValue)
                .Take(10)
                .ToList();

            // Inventario por categoría
            var inventoryByCategory = products
                .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
                .Select(g => new CategoryInventoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    ProductCount = g.Count(),
                    TotalStock = g.Sum(p => p.Stock),
                    TotalValue = g.Sum(p => p.Stock * p.Price)
                })
                .OrderByDescending(c => c.TotalValue)
                .ToList();

            return new InventoryReportDto
            {
                TotalProducts = totalProducts,
                TotalInventoryValue = totalInventoryValue,
                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts,
                LowStockItems = lowStockItems,
                TopValueProducts = topValueProducts,
                InventoryByCategory = inventoryByCategory
            };
        }

        public async Task<CustomersReportDto> GetCustomersReportAsync(ReportFilterDto filter)
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .Include(c => c.Sales)
                .ToListAsync();

            var totalCustomers = customers.Count;
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var newCustomersThisMonth = customers.Count(c => c.CreatedAt >= startOfMonth);
            var activeCustomers = customers.Count(c => c.Sales.Any(s => s.SaleDate >= filter.StartDate));

            var customerSales = customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    Customer = c,
                    TotalSpent = c.Sales.Where(s => s.SaleDate >= filter.StartDate && s.SaleDate <= filter.EndDate).Sum(s => s.TotalAmount),
                    TotalOrders = c.Sales.Count(s => s.SaleDate >= filter.StartDate && s.SaleDate <= filter.EndDate),
                    LastOrderDate = c.Sales.Any() ? c.Sales.Max(s => s.SaleDate) : DateTime.MinValue
                })
                .Where(cs => cs.TotalSpent > 0)
                .ToList();

            var averageCustomerValue = customerSales.Any() ? customerSales.Average(cs => cs.TotalSpent) : 0;

            // Top clientes
            var topCustomers = customerSales
                .OrderByDescending(cs => cs.TotalSpent)
                .Take(10)
                .Select(cs => new TopCustomerDto
                {
                    CustomerId = cs.Customer.Id,
                    CustomerName = $"{cs.Customer.FirstName} {cs.Customer.LastName}",
                    Email = cs.Customer.Email,
                    TotalOrders = cs.TotalOrders,
                    TotalSpent = cs.TotalSpent,
                    LastOrderDate = cs.LastOrderDate
                })
                .ToList();

            // Crecimiento de clientes por mes
            var customerGrowth = new List<CustomerGrowthDto>();
            var startDate = filter.StartDate;
            var currentDate = startDate;

            while (currentDate <= filter.EndDate)
            {
                var monthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var newInMonth = customers.Count(c => c.CreatedAt >= monthStart && c.CreatedAt <= monthEnd);
                var totalUpToMonth = customers.Count(c => c.CreatedAt <= monthEnd);

                customerGrowth.Add(new CustomerGrowthDto
                {
                    Month = monthStart,
                    NewCustomers = newInMonth,
                    TotalCustomers = totalUpToMonth
                });

                currentDate = currentDate.AddMonths(1);
            }

            return new CustomersReportDto
            {
                TotalCustomers = totalCustomers,
                NewCustomersThisMonth = newCustomersThisMonth,
                ActiveCustomers = activeCustomers,
                AverageCustomerValue = averageCustomerValue,
                TopCustomers = topCustomers,
                CustomerGrowth = customerGrowth
            };
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);
            var lastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            var todaySales = await _context.Sales
                .Where(s => s.SaleDate.Date == today && s.IsActive)
                .SumAsync(s => s.TotalAmount);

            var monthSales = await _context.Sales
                .Where(s => s.SaleDate >= startOfMonth && s.IsActive)
                .SumAsync(s => s.TotalAmount);

            var yearSales = await _context.Sales
                .Where(s => s.SaleDate >= startOfYear && s.IsActive)
                .SumAsync(s => s.TotalAmount);

            var lastMonthSales = await _context.Sales
                .Where(s => s.SaleDate >= lastMonth && s.SaleDate <= endOfLastMonth && s.IsActive)
                .SumAsync(s => s.TotalAmount);

            var totalOrders = await _context.Sales
                .Where(s => s.SaleDate >= startOfMonth && s.IsActive)
                .CountAsync();

            var pendingOrders = await _context.Sales
                .Where(s => s.Status == SaleStatus.Pending && s.IsActive)
                .CountAsync();

            var lowStockProducts = await _context.Products
                .Where(p => p.Stock <= 5 && p.IsActive)
                .CountAsync();

            var totalCustomers = await _context.Customers
                .Where(c => c.IsActive)
                .CountAsync();

            var growthPercentage = lastMonthSales > 0 
                ? ((monthSales - lastMonthSales) / lastMonthSales) * 100 
                : 0;

            return new DashboardSummaryDto
            {
                TodaySales = todaySales,
                MonthSales = monthSales,
                YearSales = yearSales,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                LowStockProducts = lowStockProducts,
                TotalCustomers = totalCustomers,
                GrowthPercentage = growthPercentage
            };
        }
    }
}