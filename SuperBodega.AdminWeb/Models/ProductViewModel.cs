using System.ComponentModel.DataAnnotations;

namespace SuperBodega.AdminWeb.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "La descripción es requerida")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
        
        public string ImageUrl { get; set; }
        
        [Required(ErrorMessage = "La categoría es requerida")]
        [Display(Name = "Categoría")]
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        [Required(ErrorMessage = "El proveedor es requerido")]
        [Display(Name = "Proveedor")]
        public int SupplierId { get; set; }
        
        public string SupplierName { get; set; }
    }
}