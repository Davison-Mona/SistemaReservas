using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Models
{
    public class Negocio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del negocio es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(150)]
        public string? Correo { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El identificador (Slug) es obligatorio")]
        [StringLength(50, ErrorMessage = "El slug no puede exceder los 50 caracteres")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "El slug solo puede contener letras minúsculas, números y guiones")]
        public string Slug { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        // Opcional: Propiedad de navegación hacia el modelo de Usuario (ej: ApplicationUser o IdentityUser)
        // public Usuario? Usuario { get; set; }

        // Relaciones de navegación
        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
        public ICollection<HorarioAtencion> Horarios { get; set; } = new List<HorarioAtencion>();
        public ICollection<DiaBloqueado> DiasBloqueados { get; set; } = new List<DiaBloqueado>();
    }
}