using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    public class AntecedenteMedico
    {
        [Column("id_antecedente")]
        public int IdAntecedente { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("antecedentes")]
        public string? Antecedentes { get; set; }

        [Column("alergias")]
        public string? Alergias { get; set; }

        [Column("enfermedades_cronicas")]
        public string? EnfermedadesCronicas { get; set; }

        [Column("antecedente")]
        public string Antecedente { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Column("ultima_actualizacion")]
        public DateTime UltimaActualizacion { get; set; }
    }
}
