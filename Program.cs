using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using pacientes_service.Infrastructure.MySql;
using pacientes_service.Infrastructure.MySQL;
using pacientes_service.Infrastructure.Security;

using pacientes_service.Communication.Services;

using pacientes_service.Domain.Interfaces;

using Newtonsoft.Json.Converters;
using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using pacientes_service.Infrastructure.Storage;
using Microsoft.OpenApi.Models;
using pacientes_service.Domain.Entities;
using pacientes_service.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// ---------- DB CONTEXT (usa AddDbContext, no Factory) ----------
var cs = builder.Configuration.GetConnectionString("MySql");

// Registra la FACTORY (para clases que piden IDbContextFactory<AppDbContext>)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// Registra el DbContext normal (para clases que piden AppDbContext directo)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));


// ---------- REPOS / UoW ----------
builder.Services.AddScoped<IInfrastructureFactory, MySqlInfrastructureFactory>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>(); // para refresh tokens
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>(); // <- expone SaveChangesAsync

// ---------- SERVICES (Application) ----------
builder.Services.AddScoped<IHistoriaClinicaGenerator, HistoriaClinicaGenerator>();
builder.Services.AddScoped<RegistrarPacienteService>();
builder.Services.AddScoped<IPacientesRepository, PacientesRepository>();
builder.Services.AddScoped<IInformacionMedicaInicialRepository, InformacionMedicaInicialRepository>();
builder.Services.AddScoped<UpsertInformacionMedicaInicialService>();
builder.Services.AddScoped<ListPacientesService>();
builder.Services.AddScoped<ListAntecedentesMedicosService>();
builder.Services.AddScoped<IContactoEmergenciaRepository, ContactoEmergenciaRepository>();
builder.Services.AddScoped<CreateEmergencyContactService>();
builder.Services.AddScoped<UpdateEmergencyContactService>();
builder.Services.AddScoped<CreateConsultaMedicaService>();
builder.Services.AddScoped<GetDocumentoDownloadUrlService>();
builder.Services.AddScoped<CatalogoProcedimientoService>();
builder.Services.AddScoped<CreateProcedimientoMedicoService>();

// Usuarios
builder.Services.AddScoped<CreateUserService>();
builder.Services.AddScoped<ListUsersService>();
builder.Services.AddScoped<GetUserByIdService>();
builder.Services.AddScoped<SoftDeleteUserService>();

// ---------- SECURITY / HASH / JWT ----------
builder.Services.AddSingleton<ISecurePasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>(); // usado por AuthController

const string CorsPolicy = "SpaDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
        policy
            .WithOrigins(
                "http://localhost:5173", "http://127.0.0.1:5173",
                "http://localhost:4173", "http://127.0.0.1:4173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// JWT (opcional: comenta esta sección si aún no usarás login)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            NameClaimType = "username",
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

// ---------- S3 ----------
builder.Services.Configure<S3Options>(builder.Configuration.GetSection("S3"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var awsAccessKey = builder.Configuration["AWS:AccessKeyId"];
    var awsSecretKey = builder.Configuration["AWS:SecretAccessKey"];
    var regionName = builder.Configuration["S3:Region"] ?? "us-east-1";
    var creds = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
    return new AmazonS3Client(creds, RegionEndpoint.GetBySystemName(regionName));
});
builder.Services.AddScoped<IS3StorageService, S3StorageService>();
builder.Services.AddScoped<UploadDocumentoDigitalizadoService>();

// ---------- MVC / JSON ----------
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });

    // Bearer en Swagger (para probar endpoints protegidos)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT en formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme, Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------- PIPELINE ----------
app.UseHttpsRedirection();

// Si usas JWT:
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors(CorsPolicy);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<ISecurePasswordHasher>();

    var admin = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == "admin");
    var newPlain = "Admin#123"; // cámbialo si quieres

    if (admin == null)
    {
        db.Usuarios.Add(new Usuario
        {
            NombreUsuario = "admin",
            Contrasena = hasher.Hash(newPlain),
            Rol = "administrador",
            IdMedico = null
        });
        db.SaveChanges();
        Console.WriteLine(">>> Admin creado: admin / " + newPlain);
    }
    else if (string.IsNullOrEmpty(admin.Contrasena) || !admin.Contrasena.StartsWith("$2"))
    {
        admin.Contrasena = hasher.Hash(newPlain);
        db.SaveChanges();
        Console.WriteLine(">>> Admin actualizado con password (hash) nuevo.");
    }
}

app.Run();
