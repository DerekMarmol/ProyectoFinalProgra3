using System;
using System.ComponentModel.DataAnnotations;

namespace SuperBodega.AdminWeb.Models.Reports
{
    public class ReportFilterViewModel
    {
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Categoría")]
        public int? CategoryId { get; set; }

        [Display(Name = "Proveedor")]
        public int? SupplierId { get; set; }

        [Display(Name = "Cliente")]
        public int? CustomerId { get; set; }

        [Display(Name = "Período")]
        public ReportPeriod Period { get; set; } = ReportPeriod.Last30Days;
    }

    public enum ReportPeriod
    {
        [Display(Name = "Hoy")]
        Today,
        
        [Display(Name = "Ayer")]
        Yesterday,
        
        [Display(Name = "Últimos 7 días")]
        Last7Days,
        
        [Display(Name = "Últimos 30 días")]
        Last30Days,
        
        [Display(Name = "Mes pasado")]
        LastMonth,
        
        [Display(Name = "Últimos 3 meses")]
        Last3Months,
        
        [Display(Name = "Últimos 6 meses")]
        Last6Months,
        
        [Display(Name = "Último año")]
        LastYear,
        
        [Display(Name = "Personalizado")]
        Custom
    }
}