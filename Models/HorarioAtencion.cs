namespace SistemaReservas.Models
{
    public class HorarioAtencion
    {
        public int Id { get; set; }
        public DayOfWeek DiaSemana { get; set; } // Monday, Tuesday...
        public TimeSpan HoraApertura { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan HoraCierre { get; set; } = new TimeSpan(18, 0, 0);
        public bool Activo { get; set; } = true;

        public int NegocioId { get; set; }
        public Negocio? Negocio { get; set; }
    }
}