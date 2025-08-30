using pacientes_service.Domain.Entities;

namespace pacientes_service.Communication.Commands
{
    public class RegistrarPacienteCommand
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Dpi { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Sexo { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? EstadoCivil { get; set; }
    }
}
