namespace pacientes_service.Infrastructure.Storage
{
    public class S3Options
    {
        public string Bucket { get; set; } = "";
        public string Region { get; set; } = "us-east-1";
        public string BaseFolder { get; set; } = "adjuntos";
    }
}
