using System.ComponentModel.DataAnnotations;

namespace pacientes_service.Communication.Contracts
{
    public class UploadDocumentoForm
    {
        [Required] public IFormFile File { get; set; } = default!;
        [Required] public string Categoria { get; set; } = "";
        public string? Notas { get; set; }
    }
}
