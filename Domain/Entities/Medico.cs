using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace pacientes_service.Domain.Entities
{
    public class Medico
    {
        [Key]
        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Required]
        [Column("nombres")]
        [MaxLength(150)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [Column("apellidos")]
        [MaxLength(150)]
        public string Apellidos { get; set; } = string.Empty;

        [Column("numero_colegiado")]
        [MaxLength(50)]
        public string? NumeroColegiado { get; set; }

        [Required]
        [Column("especialidad")]
        [MaxLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Column("telefono")]
        [MaxLength(20)]
        public string? Telefono { get; set; }

        [Column("correo")]
        [MaxLength(100)]
        public string? Correo { get; set; }

        [Column("horario_laboral")]
        public string? HorarioLaboral { get; set; }
    }
}
