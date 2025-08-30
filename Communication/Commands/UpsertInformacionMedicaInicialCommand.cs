namespace pacientes_service.Communication.Commands
{
    public class UpsertInformacionMedicaInicialCommand
    {
        public int IdPaciente { get; set; }
        public string? Antecedentes { get; set; }
        public string? Alergias { get; set; }
        public string? EnfermedadesCronicas { get; set; }
    }
}
