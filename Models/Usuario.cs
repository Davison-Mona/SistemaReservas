namespace SistemaReservas.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty; 
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        // Campos para el token de recuperación
        public string? TokenRestablecimiento { get; set; }
        public DateTime? TokenExpiracion { get; set; }
    }
}