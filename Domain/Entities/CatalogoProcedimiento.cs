namespace pacientes_service.Domain.Entities
{
    public class CatalogoProcedimiento
    {
        public int IdProcedimientoCatalogo { get; set; }
        public string Codigo { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public decimal PrecioBase { get; set; }
        public int? DuracionMin { get; set; }
        public bool Activo { get; set; } = true;
    }
}
