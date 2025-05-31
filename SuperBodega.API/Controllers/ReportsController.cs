// SuperBodega.API/Controllers/ReportsController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.Core.DTOs.Reports;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
        {
            var summary = await _reportService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpPost("sales")]
        public async Task<ActionResult<SalesReportDto>> GetSalesReport([FromBody] ReportFilterDto filter)
        {
            if (filter == null)
            {
                filter = new ReportFilterDto();
            }

            // Ajustar fechas según el período seleccionado
            AdjustDatesForPeriod(filter);

            var report = await _reportService.GetSalesReportAsync(filter);
            return Ok(report);
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
        {
            var report = await _reportService.GetInventoryReportAsync();
            return Ok(report);
        }

        [HttpPost("customers")]
        public async Task<ActionResult<CustomersReportDto>> GetCustomersReport([FromBody] ReportFilterDto filter)
        {
            if (filter == null)
            {
                filter = new ReportFilterDto();
            }

            AdjustDatesForPeriod(filter);

            var report = await _reportService.GetCustomersReportAsync(filter);
            return Ok(report);
        }

        [HttpGet("export/sales")]
        public async Task<IActionResult> ExportSalesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var filter = new ReportFilterDto
            {
                StartDate = startDate ?? DateTime.Today.AddDays(-30),
                EndDate = endDate ?? DateTime.Today
            };

            var report = await _reportService.GetSalesReportAsync(filter);
            
            // Aquí podrías implementar la exportación a Excel/PDF
            // Por ahora devolvemos JSON
            return Ok(report);
        }

        private void AdjustDatesForPeriod(ReportFilterDto filter)
        {
            var today = DateTime.Today;
            
            switch (filter.Period)
            {
                case ReportPeriod.Today:
                    filter.StartDate = today;
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.Yesterday:
                    filter.StartDate = today.AddDays(-1);
                    filter.EndDate = today.AddDays(-1);
                    break;
                
                case ReportPeriod.Last7Days:
                    filter.StartDate = today.AddDays(-7);
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.Last30Days:
                    filter.StartDate = today.AddDays(-30);
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.LastMonth:
                    var firstDayLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                    filter.StartDate = firstDayLastMonth;
                    filter.EndDate = firstDayLastMonth.AddMonths(1).AddDays(-1);
                    break;
                
                case ReportPeriod.Last3Months:
                    filter.StartDate = today.AddMonths(-3);
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.Last6Months:
                    filter.StartDate = today.AddMonths(-6);
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.LastYear:
                    filter.StartDate = today.AddYears(-1);
                    filter.EndDate = today;
                    break;
                
                case ReportPeriod.Custom:
                    // Las fechas ya están establecidas
                    break;
            }
        }
    }
}