using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = null!;

        [Column("contrasena")]
        public string Contrasena { get; set; } = null!;

        [Column("rol")]
        public string Rol { get; set; } = null!;

        [Column("id_medico")]
        public int? IdMedico { get; set; }

        [Column("id_paciente")]
        public int? IdPaciente { get; internal set; }
    }
}
