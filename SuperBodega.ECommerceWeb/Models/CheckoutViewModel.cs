using System.ComponentModel.DataAnnotations;

namespace SuperBodega.ECommerceWeb.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Por favor ingrese su nombre")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Por favor ingrese su apellido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Por favor ingrese su email")]
        [EmailAddress(ErrorMessage = "Por favor ingrese un email válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor ingrese su teléfono")]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Por favor ingrese su dirección")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }
    }
}