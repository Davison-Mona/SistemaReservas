using System.ComponentModel.DataAnnotations;

namespace SistemaReservas.Models
{
    public class DiaBloqueado
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public int NegocioId { get; set; }
        public Negocio? Negocio { get; set; }
    }
}