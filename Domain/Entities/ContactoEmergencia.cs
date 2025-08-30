using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    public class ContactoEmergencia
    {
        [Column("id_contacto")]
        public int IdContacto { get; set; }
        [Column("id_paciente")]
        public int IdPaciente { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; } = null!;
        [Column("parentesco")]
        public string Parentesco { get; set; } = null!;
        [Column("telefono")]
        public string Telefono { get; set; } = null!;
        public Paciente Paciente { get; set; } = null!;
    }
}
