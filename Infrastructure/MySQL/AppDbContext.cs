using Microsoft.EntityFrameworkCore;
using pacientes_service.Domain.Entities;

namespace pacientes_service.Infrastructure.MySql;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
    public DbSet<ContactoEmergencia> ContactosEmergencia { get; set; }
    public DbSet<AntecedenteMedico> AntecedentesMedicos { get; set; } = null!;
    public DbSet<Medico> Medicos { get; set; } = null!;
    public DbSet<Imagenologia> Imagenologia { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<CitaMedica> CitasMedicas { get; set; } = null!;
    public DbSet<ConsultaMedica> ConsultasMedicas { get; set; } = null!;
    public DbSet<ProcedimientoMedico> ProcedimientosMedicos => Set<ProcedimientoMedico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paciente>()
            .HasKey(p => p.IdPaciente);

        modelBuilder.Entity<ContactoEmergencia>()
            .HasKey(c => c.IdContacto);

        modelBuilder.Entity<ContactoEmergencia>()
            .HasOne(c => c.Paciente)
            .WithMany(p => p.ContactosEmergencia)
            .HasForeignKey(c => c.IdPaciente);

        modelBuilder.Entity<HistoriaClinica>(b =>
        {
            b.ToTable("HistoriasClinicas");
            b.HasKey(h => h.IdHistoriaClinica);
            b.Property(h => h.IdHistoriaClinica).HasColumnName("id_historia_clinica");
            b.Property(h => h.IdPaciente).HasColumnName("id_paciente");
            b.Property(h => h.NumeroHistoriaClinica).HasColumnName("numero_historia_clinica");
            b.Property(h => h.FechaRegistro)
                .HasColumnName("fecha_registro")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            b.Property(h => h.Descripcion).HasColumnName("descripcion");
            b.Property(h => h.TipoRegistro)
                .HasColumnName("tipo_registro")
                .HasMaxLength(20)
                .HasDefaultValue("historia");
            b.Property(h => h.IdMedico)
                .HasColumnName("id_medico");
            b.Property(h => h.Fecha)
                .HasColumnName("fecha");
            b.Property(h => h.MotivoConsulta)
                .HasColumnName("motivo_consulta");
            b.Property(h => h.Diagnostico)
                .HasColumnName("diagnostico");
            b.HasOne(h => h.Paciente)
             .WithMany(p => p.HistoriasClinicas)
             .HasForeignKey(h => h.IdPaciente)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("fk_historia_paciente");
            b.HasOne<Medico>()
             .WithMany()
             .HasForeignKey(h => h.IdMedico)
             .OnDelete(DeleteBehavior.SetNull)
             .HasConstraintName("fk_historia_medico");
            b.HasIndex(h => h.IdPaciente).HasDatabaseName("idx_historia_paciente");
            b.HasIndex(h => h.FechaRegistro).HasDatabaseName("idx_historia_fecha");
            b.HasIndex(h => h.TipoRegistro).HasDatabaseName("idx_historia_tipo");
            b.HasIndex(h => h.IdMedico).HasDatabaseName("idx_historia_medico");
            b.HasIndex(h => h.Fecha).HasDatabaseName("idx_historia_fecha2");
        });

        modelBuilder.Entity<AntecedenteMedico>(b =>
        {
            b.ToTable("AntecedentesMedicos");
            b.HasKey(a => a.IdAntecedente);
            b.Property(a => a.IdAntecedente).HasColumnName("id_antecedente");
            b.Property(a => a.IdPaciente).HasColumnName("id_paciente");
            b.Property(a => a.Antecedentes).HasColumnName("antecedentes");
            b.Property(a => a.Alergias).HasColumnName("alergias");
            b.Property(a => a.EnfermedadesCronicas).HasColumnName("enfermedades_cronicas");
            b.Property(a => a.Antecedente).HasColumnName("antecedente");
            b.Property(a => a.Descripcion).HasColumnName("descripcion");
            b.Property(a => a.FechaRegistro)
             .HasColumnName("fecha_registro")
             .HasDefaultValueSql("CURRENT_TIMESTAMP");
            b.Property(a => a.UltimaActualizacion)
             .HasColumnName("ultima_actualizacion")
             .HasDefaultValueSql("CURRENT_TIMESTAMP")
             .ValueGeneratedOnAddOrUpdate();
            b.HasIndex(a => a.IdPaciente).HasDatabaseName("idx_ant_paciente");
        });

        modelBuilder.Entity<Medico>(b =>
        {
            b.ToTable("Medicos");
            b.HasKey(m => m.IdMedico);
            b.Property(m => m.IdMedico).HasColumnName("id_medico");
            b.Property(m => m.Nombres).HasColumnName("nombres").HasMaxLength(150).IsRequired();
            b.Property(m => m.Apellidos).HasColumnName("apellidos").HasMaxLength(150).IsRequired();
            b.Property(m => m.NumeroColegiado).HasColumnName("numero_colegiado").HasMaxLength(50);
            b.Property(m => m.Especialidad).HasColumnName("especialidad").HasMaxLength(100).IsRequired();
            b.Property(m => m.Telefono).HasColumnName("telefono").HasMaxLength(20);
            b.Property(m => m.Correo).HasColumnName("correo").HasMaxLength(100);
            b.Property(m => m.HorarioLaboral).HasColumnName("horario_laboral");
        });

        modelBuilder.Entity<Imagenologia>(b =>
        {
            b.ToTable("Imagenologia");
            b.HasKey(i => i.IdImagen);

            b.Property(i => i.IdImagen).HasColumnName("id_imagen");
            b.Property(i => i.IdPaciente).HasColumnName("id_paciente");
            b.Property(i => i.TipoImagen).HasColumnName("tipo_imagen");
            b.Property(i => i.ImagenUrl).HasColumnName("imagen_url");
            b.Property(i => i.FechaEstudio).HasColumnName("fecha_estudio");
            b.Property(i => i.MedicoSolicitante).HasColumnName("medico_solicitante");
            b.Property(i => i.Categoria).HasColumnName("categoria");
            b.Property(i => i.S3Bucket).HasColumnName("s3_bucket");
            b.Property(i => i.S3Key).HasColumnName("s3_key");
            b.Property(i => i.ContentType).HasColumnName("content_type");
            b.Property(i => i.TamanoBytes).HasColumnName("tamano_bytes");
            b.Property(i => i.Notas).HasColumnName("notas");
            b.Property(i => i.FechaDocumento).HasColumnName("fecha_documento");
            b.Property(i => i.NombreArchivoOriginal).HasColumnName("nombre_archivo_original");
            b.HasIndex(i => i.IdPaciente).HasDatabaseName("idx_img_paciente");
            b.HasIndex(i => i.FechaEstudio).HasDatabaseName("idx_img_fecha");
            b.HasIndex(i => i.Categoria).HasDatabaseName("idx_img_categoria");
        });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.ToTable("Usuarios");
            b.HasKey(u => u.IdUsuario);

            b.Property(u => u.IdUsuario).HasColumnName("id_usuario");
            b.Property(u => u.NombreUsuario).HasColumnName("nombre_usuario").IsRequired();
            b.Property(u => u.Contrasena).HasColumnName("contrasena").IsRequired();
            b.Property(u => u.Rol).HasColumnName("rol").IsRequired();
            b.Property(u => u.IdMedico).HasColumnName("id_medico");
            b.Property(x => x.IdPaciente).HasColumnName("id_paciente");
            b.Property<bool>("eliminado").HasColumnName("eliminado").HasDefaultValue(false);
            b.Property<DateTime?>("eliminado_en").HasColumnName("eliminado_en");
            b.Property<string?>("eliminado_por").HasColumnName("eliminado_por");
            b.HasIndex(u => u.NombreUsuario).HasDatabaseName("uq_usuarios_nombre").IsUnique();
            b.HasOne<Medico>()
             .WithMany()
             .HasForeignKey(u => u.IdMedico)
             .OnDelete(DeleteBehavior.SetNull)
             .HasConstraintName("fk_usuarios_medicos");
            b.HasQueryFilter(u => EF.Property<bool>(u, "eliminado") == false);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshTokens");
            b.HasKey(x => x.IdRefresh);
            b.Property(x => x.IdRefresh).HasColumnName("id_refresh");
            b.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            b.Property(x => x.TokenHash).HasColumnName("token_hash");
            b.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            b.Property(x => x.CreatedAt).HasColumnName("created_at");
            b.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        });

        modelBuilder.Entity<CitaMedica>(e =>
        {
            e.ToTable("citasmedicas");
            e.HasKey(x => x.IdCita);
            e.Property(x => x.IdCita).HasColumnName("id_cita");
            e.Property(x => x.IdPaciente).HasColumnName("id_paciente");
            e.Property(x => x.IdMedico).HasColumnName("id_medico");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.Property(x => x.Estado).HasColumnName("estado");
            e.Property(x => x.RazonCancelacion).HasColumnName("razon_cancelacion");
        });

        modelBuilder.Entity<ConsultaMedica>(e =>
        {
            e.ToTable("consultasmedicas");
            e.HasKey(x => x.IdConsulta);
            e.Property(x => x.IdConsulta).HasColumnName("id_consulta");
            e.Property(x => x.IdPaciente).HasColumnName("id_paciente");
            e.Property(x => x.IdMedico).HasColumnName("id_medico");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.Property(x => x.MotivoConsulta).HasColumnName("motivo_consulta");
            e.Property(x => x.Diagnostico).HasColumnName("diagnostico");
            e.Property(x => x.Observaciones).HasColumnName("observaciones");

            e.Property(x => x.IdCita).HasColumnName("id_cita");
            e.HasOne<CitaMedica>()
             .WithMany()
             .HasForeignKey(x => x.IdCita)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProcedimientoMedico>(e =>
        {
            e.ToTable("procedimientosmedicos");
            e.HasKey(x => x.IdProcedimiento);
            e.Property(x => x.IdProcedimiento).HasColumnName("id_procedimiento");
            e.Property(x => x.IdConsulta).HasColumnName("id_consulta");
            e.Property(x => x.IdPaciente).HasColumnName("id_paciente");
            e.Property(x => x.IdMedico).HasColumnName("id_medico");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.Property(x => x.Tipo).HasColumnName("tipo");
            e.Property(x => x.Descripcion).HasColumnName("descripcion");
            e.Property(x => x.Costo).HasColumnName("costo");
            e.Property(x => x.Estado).HasColumnName("estado");
        });
    }
}