using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{
    [Table("Imagenologia")]
    public class Imagenologia
    {
        [Column("id_imagen")]
        public int IdImagen { get; set; }
        [Column("id_paciente")] 
        public int IdPaciente { get; set; }
        [Column("tipo_imagen")] 
        public string TipoImagen { get; set; } = "documento";
        [Column("imagen_url")] 
        public string ImagenUrl { get; set; } = "";
        [Column("fecha_estudio")] 
        public DateTime FechaEstudio { get; set; }
        [Column("medico_solicitante")] 
        public int? MedicoSolicitante { get; set; }
        [Column("categoria")] 
        public string Categoria { get; set; } = "otro";
        [Column("s3_bucket")] 
        public string? S3Bucket { get; set; }
        [Column("s3_key")] 
        public string? S3Key { get; set; }
        [Column("content_type")]
        public string? ContentType { get; set; }
        [Column("tamano_bytes")]
        public long? TamanoBytes { get; set; }
        [Column("notas")]
        public string? Notas { get; set; }
        [Column("fecha_documento")]
        public DateTime? FechaDocumento { get; set; }
        [Column("nombre_archivo_original")]
        public string? NombreArchivoOriginal { get; set; }
    }
}
