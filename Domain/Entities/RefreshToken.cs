using System.ComponentModel.DataAnnotations.Schema;

namespace pacientes_service.Domain.Entities
{

    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Column("id_refresh")] public int IdRefresh { get; set; }
        [Column("id_usuario")] public int IdUsuario { get; set; }
        [Column("token_hash")] public string TokenHash { get; set; } = null!;
        [Column("expires_at")] public DateTime ExpiresAt { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }
        [Column("revoked_at")] public DateTime? RevokedAt { get; set; }
    }
}
