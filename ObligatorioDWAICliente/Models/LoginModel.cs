using System.ComponentModel.DataAnnotations;

namespace ObligatorioDWAICliente.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasenia { get; set; } = string.Empty;
    }
}
