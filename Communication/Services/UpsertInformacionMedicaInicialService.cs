// Communication/Services/UpsertInformacionMedicaInicialService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pacientes_service.Communication.Commands;
using pacientes_service.Domain.Entities;
using pacientes_service.Domain.Interfaces;
using pacientes_service.Infrastructure.MySql; // AppDbContext

namespace pacientes_service.Communication.Services
{
    public class UpsertInformacionMedicaInicialService
    {
        private readonly IInformacionMedicaInicialRepository _repo;
        private readonly AppDbContext _db;
        private readonly IUnitOfWork? _uow; // opcional

        public UpsertInformacionMedicaInicialService(
            IInformacionMedicaInicialRepository repo,
            AppDbContext db,
            IUnitOfWork? uow = null) // opcional
        {
            _repo = repo;
            _db = db;
            _uow = uow;
        }

        public async Task<int> HandleAsync(
            UpsertInformacionMedicaInicialCommand cmd,
            CancellationToken ct = default)
        {
            // 1) Validar existencia de paciente con DbContext directo (opción B)
            var existePaciente = await _db.Pacientes
                .AnyAsync(p => p.IdPaciente == cmd.IdPaciente, ct);

            if (!existePaciente)
                throw new ArgumentException("Paciente no existe.");

            // 2) Tomar el registro a actualizar (o el último si aún no hay “ficha”)
            // (Los repos de ejemplo no aceptan CT; si quieres, agrega overloads con ct)
            var reg = await _repo.GetRegistroParaUpsertAsync(cmd.IdPaciente);

            if (reg == null)
            {
                reg = new AntecedenteMedico
                {
                    IdPaciente = cmd.IdPaciente,
                    Antecedentes = cmd.Antecedentes?.Trim(),
                    Alergias = cmd.Alergias?.Trim(),
                    EnfermedadesCronicas = cmd.EnfermedadesCronicas?.Trim(),

                    // legacy requeridos por la tabla original
                    Antecedente = string.Empty,
                    Descripcion = null,

                    // timestamps (DB tiene defaults; aquí dejamos valores razonables)
                    FechaRegistro = DateTime.UtcNow,
                    UltimaActualizacion = DateTime.UtcNow
                };

                await _repo.AddAsync(reg);
            }
            else
            {
                reg.Antecedentes = cmd.Antecedentes?.Trim();
                reg.Alergias = cmd.Alergias?.Trim();
                reg.EnfermedadesCronicas = cmd.EnfermedadesCronicas?.Trim();
                reg.UltimaActualizacion = DateTime.UtcNow;

                await _repo.UpdateAsync(reg);
            }

            return reg.IdAntecedente;
        }
    }
}
