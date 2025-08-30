using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    public class HistoriaClinica
    {
        [Column("id_historia_clinica")] 
        public int IdHistoriaClinica { get; set; }
        [Column("id_paciente")] 
        public int IdPaciente { get; set; }
        [Column("id_medico")]
        public int? IdMedico { get; set; }
        [Column("numero_historia_clinica")] 
        public string NumeroHistoriaClinica { get; set; } = null!;
        [Column("tipo_registro")] 
        public string TipoRegistro { get; set; } = "historia";
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        [Column("fecha")] 
        public DateTime? Fecha { get; set; }
        [Column("motivo_consulta")] 
        public string? MotivoConsulta { get; set; }
        [Column("diagnostico")] 
        public string? Diagnostico { get; set; }
        [Column("descripcion")] 
        public string Descripcion { get; set; } = null!;
        public Paciente Paciente { get; set; } = null!;
    }
}
