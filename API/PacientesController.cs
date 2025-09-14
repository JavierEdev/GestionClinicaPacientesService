using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pacientes_service.Communication.Commands;
using pacientes_service.Communication.Contracts;
using pacientes_service.Communication.Services;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql;
using pacientes_service.Infrastructure.MySQL;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly RegistrarPacienteService _svc;
    private readonly IPacientesRepository _repo;

    public PacientesController(RegistrarPacienteService svc, IPacientesRepository repo)
    {
        _svc = svc;
        _repo = repo;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegistrarPacienteCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var (id, hc) = await _svc.HandleAsync(cmd, ct);
            return CreatedAtAction(nameof(GetById),
                new { idPaciente = id },
                new { id});
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("DPI ya está registrado"))
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{idPaciente:int}")]
    public async Task<IActionResult> GetById(int idPaciente, CancellationToken ct)
    {
        var paciente = await _repo.GetByIdAsync(idPaciente, ct);

        if (paciente == null)
            return NotFound(new { error = "Paciente no encontrado" });

        return Ok(new
        {
            paciente.IdPaciente,
            paciente.Nombres,
            paciente.Apellidos,
            paciente.Dpi,
            paciente.FechaNacimiento,
            paciente.Sexo,
            paciente.Direccion,
            paciente.Telefono,
            paciente.Correo,
            paciente.EstadoCivil,
            ContactosEmergencia = paciente.ContactosEmergencia.Select(c => new
            {
                c.IdContacto,
                c.Nombre,
                c.Parentesco,
                c.Telefono
            })
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetPacientes(
    [FromServices] pacientes_service.Communication.Services.ListPacientesService svc,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50,
    CancellationToken ct = default)
    {
        var result = await svc.HandleAsync(page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("pacientes/{idPaciente:int}/antecedentes-medicos")]
    public async Task<IActionResult> GetAntecedentesMedicosDePaciente(
        [FromServices] pacientes_service.Communication.Services.ListAntecedentesMedicosService svc,
        [FromRoute] int idPaciente,
        CancellationToken ct = default)
    {
        var item = await svc.GetByPacienteAsync(idPaciente, ct);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPut("{idPaciente:int}/informacion-medica-inicial")]
    public async Task<IActionResult> UpsertInformacionMedicaInicial(
    int idPaciente,
    [FromBody] UpsertInformacionMedicaInicialCommand body,
    [FromServices] UpsertInformacionMedicaInicialService service,
    CancellationToken ct)
    {
        if (idPaciente != body.IdPaciente)
            return BadRequest("idPaciente de la ruta no coincide con el del cuerpo.");

        var id = await service.HandleAsync(body, ct);
        return Ok(new { idAntecedente = id, idPaciente });
    }

    [HttpGet("{idPaciente:int}/contactos")]
    public async Task<IActionResult> GetContactos(
    int idPaciente,
        [FromServices] IContactoEmergenciaRepository repo,
        CancellationToken ct = default)
    {
        var items = await repo.ListByPacienteAsync(idPaciente, ct);
        return Ok(items.Select(c => new {
            c.IdContacto,
            c.IdPaciente,
            c.Nombre,
            c.Parentesco,
            c.Telefono
        }));
    }

    [HttpPost("{idPaciente:int}/contactos")]
    public async Task<IActionResult> CreateContacto(
        int idPaciente,
        [FromBody] CreateEmergencyContactCommand cmd,
        [FromServices] CreateEmergencyContactService svc,
        CancellationToken ct = default)
    {
        if (idPaciente != cmd.IdPaciente)
            return BadRequest("idPaciente de la ruta no coincide con el del cuerpo.");

        var created = await svc.HandleAsync(cmd, ct);

        return CreatedAtAction(nameof(GetContactos),
            new { idPaciente = created.IdPaciente },
            new
            {
                created.IdContacto,
                created.IdPaciente,
                created.Nombre,
                created.Parentesco,
                created.Telefono
            });
    }

    [HttpPut("{idPaciente:int}/contactos/{idContacto:int}")]
    public async Task<IActionResult> UpdateContacto(
        int idPaciente,
        int idContacto,
        [FromBody] UpdateEmergencyContactCommand cmd,
        [FromServices] UpdateEmergencyContactService svc,
        CancellationToken ct = default)
    {
        if (idPaciente != cmd.IdPaciente || idContacto != cmd.IdContacto)
            return BadRequest("Los ids de la ruta no coinciden con el cuerpo.");

        await svc.HandleAsync(cmd, ct);
        return NoContent();
    }

    [HttpPost("{idPaciente:int}/consultas")]
    public async Task<IActionResult> CreateConsulta(
        int idPaciente,
        [FromBody] CreateConsultaMedicaCommand cmd,
        [FromServices] CreateConsultaMedicaService svc,
        CancellationToken ct = default)
    {
        if (idPaciente != cmd.IdPaciente)
            return BadRequest("idPaciente de la ruta no coincide con el del cuerpo.");

        var idConsulta = await svc.HandleAsync(cmd, ct);

        return CreatedAtAction(nameof(GetConsultasPaciente),
            new { idPaciente },
            new
            {
                idConsulta,
                idPaciente,
                cmd.IdMedico,
                cmd.IdCita,
                cmd.Fecha,
                cmd.MotivoConsulta,
                cmd.Diagnostico,
                cmd.Observaciones
            });
    }

    [HttpGet("{idPaciente:int}/consultas")]
    public async Task<IActionResult> GetConsultasPaciente(
        int idPaciente,
        [FromServices] AppDbContext db,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 200) pageSize = 50;

        var q = db.ConsultasMedicas.AsNoTracking()
            .Where(c => c.IdPaciente == idPaciente)
            .OrderByDescending(c => c.Fecha);

        var total = await q.CountAsync(ct);

        var items = await q.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.IdConsulta,
                c.IdPaciente,
                c.IdMedico,
                c.IdCita,
                c.Fecha,
                c.MotivoConsulta,
                c.Diagnostico,
                c.Observaciones
            })
            .ToListAsync(ct);

        return Ok(new { idPaciente, page, pageSize, total, items });
    }

    [HttpPost("{idPaciente:int}/documentos")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> SubirDocumentoDigitalizado(
        int idPaciente,
        [FromForm] UploadDocumentoForm form,
        [FromServices] UploadDocumentoDigitalizadoService svc,
        CancellationToken ct = default)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest("Archivo requerido.");

        var idImagen = await svc.HandleAsync(idPaciente, form.Categoria, form.File, form.Notas, ct);
        return CreatedAtAction(nameof(ListarDocumentosDigitalizados),
            new { idPaciente },
            new { idImagen, idPaciente, categoria = form.Categoria, nombreArchivo = form.File.FileName });
    }

    [HttpGet("{idPaciente:int}/documentos")]
    public async Task<IActionResult> ListarDocumentosDigitalizados(
        int idPaciente,
        [FromServices] AppDbContext db,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 200) pageSize = 50;

        var q = db.Imagenologia.AsNoTracking()
            .Where(x => x.IdPaciente == idPaciente &&
                        (x.Categoria == "dpi" || x.Categoria == "resultado" || x.Categoria == "seguro"))
            .OrderByDescending(x => x.FechaDocumento ?? x.FechaEstudio);

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new {
                x.IdImagen,
                x.Categoria,
                x.ContentType,
                x.TamanoBytes,
                x.FechaDocumento,
                x.FechaEstudio,
                x.NombreArchivoOriginal,
                x.Notas
            })
            .ToListAsync(ct);

        return Ok(new { idPaciente, page, pageSize, total, items });
    }

    [HttpGet("{idPaciente:int}/documentos/{idImagen:int}/download")]
    public async Task<IActionResult> GetDocumentoDownloadUrl(
        int idPaciente,
        int idImagen,
        [FromServices] GetDocumentoDownloadUrlService svc,
        [FromQuery] int ttlSeconds = 300,
        CancellationToken ct = default)
    {
        if (ttlSeconds <= 0 || ttlSeconds > 3600) ttlSeconds = 300;
        try
        {
            var url = await svc.HandleAsync(idPaciente, idImagen, TimeSpan.FromSeconds(ttlSeconds), ct);
            return Ok(new { url, expiresIn = ttlSeconds });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("consultas/{idConsulta:int}/procedimientos")]
    public async Task<IActionResult> CreateProcedimiento(
        [FromBody] CreateProcedimientoMedicoCommand cmd,
        [FromServices] CreateProcedimientoMedicoService svc,
        CancellationToken ct = default)
    {

        // Crear el procedimiento y obtener el ID del procedimiento y la consulta
        var (idProcedimiento, idConsulta) = await svc.HandleAsync(cmd, ct);

        return Ok(new { idProcedimiento, idConsulta });
    }



    [HttpGet("{idPaciente:int}/consultas/{idConsulta:int}/procedimientos")]
    public async Task<IActionResult> ListProcedimientosDeConsulta(
        int idPaciente,
        int idConsulta,
        [FromServices] AppDbContext db,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 200) pageSize = 50;

        var q = db.ProcedimientosMedicos.AsNoTracking()
            .Where(p => p.IdConsulta == idConsulta)
            .OrderByDescending(p => p.IdProcedimiento);

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new {
                p.IdProcedimiento,
                p.IdConsulta,
                p.Procedimiento,
                p.Descripcion
            })
            .ToListAsync(ct);

        return Ok(new { idConsulta, page, pageSize, total, items });
    }


}
