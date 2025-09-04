using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities;

public class Paciente
{
    [Column("id_paciente")]
    public int IdPaciente { get; set; }
    [Column("nombres")]
    public string Nombres { get; set; } = null!;
    [Column("apellidos")]
    public string Apellidos { get; set; } = null!;
    [Column("dpi")]
    public string Dpi { get; set; } = null!;
    [Column("fecha_nacimiento")]
    public DateTime FechaNacimiento { get; set; }
    [Column("sexo")]
    public required string Sexo { get; set; }
    [Column("direccion")]
    public string? Direccion { get; set; } = null!;
    [Column("telefono")]
    public string Telefono { get; set; } = null!;
    [Column("correo")]
    public string? Correo { get; set; } = null!;
    [Column("estado_civil")]
    public string EstadoCivil { get; set; } = null!;
    public ICollection<ContactoEmergencia> ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
    public ICollection<HistoriaClinica> HistoriasClinicas { get; set; } = new List<HistoriaClinica>();
}
