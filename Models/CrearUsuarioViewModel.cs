using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Models
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string Rol { get; set; } = string.Empty; // "Admin", "Cliente", "Negocio", etc.
    }
}