using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    [Table("procedimientosmedicos")]
    public class ProcedimientoMedico
    {
        [Column("id_procedimiento")]
        public int IdProcedimiento { get; set; }

        [Column("id_consulta")]
        public int IdConsulta { get; set; }

        [Column("id_procedimiento_catalogo")]
        public int IdProcedimientoCatalogo { get; set; }

        [Column("procedimiento")]
        public string Procedimiento { get; set; } = default!;

        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }
}
