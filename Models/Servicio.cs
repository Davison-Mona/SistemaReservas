using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaReservas.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int DuracionMinutos { get; set; } = 30;

        public int NegocioId { get; set; }
        public Negocio? Negocio { get; set; }
    }
}