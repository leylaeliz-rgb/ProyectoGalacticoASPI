using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Dtos.Responses;
using ProyectoGalactico.Models;
using Microsoft.AspNetCore.Identity;
using ProyectoGalactico.Enums;
using Microsoft.EntityFrameworkCore;

namespace ProyectoGalactico.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Event> _hasher;
        private readonly IEncryptionService _encryption;

        public EventService (AppDbContext db, IPasswordHasher<Event> hasher, IEncryptionService encryption)
        {
            _db = db;
            _hasher = hasher;
            _encryption = encryption;
        }

        public async Task<ServiceResult<List<EventDto>>> GetAllAsync(int? characterId, string? from, string? to)
        {
            int fromYear = 0, toYear = 0;
            if (from is not null && !YearConverter.TryParse(from, out fromYear))
                return ServiceResult<List<EventDto>>.Invalid("'desde' no es una fecha válida (ej. 10 BBY, 3 ABY).");
            if (to is not null && !YearConverter.TryParse(to, out toYear))
                return ServiceResult<List<EventDto>>.Invalid("'hasta' no es una fecha válida (ej. 10 BBY, 3 ABY).");
            if (from is not null && to is not null && fromYear > toYear)
                return ServiceResult<List<EventDto>>.Invalid("'desde' no puede ser posterior a 'hasta'.");

            var events = await _db.Events.AsNoTracking().OrderBy(e => e.Year).ThenBy(e => e.Id).ToListAsync();
            var filtered = events.Where(e =>
                (!characterId.HasValue || e.ParticipantsIds.Contains(characterId.Value)) &&
                (from is null || e.Year >= fromYear) &&
                (to is null || e.Year <= toYear));

            return ServiceResult<List<EventDto>>.Ok(filtered.Select(ADto).ToList());
        }

        public async Task<ServiceResult<EventDto>> GetByIdAsync(int id)
        {
            var eventEntity = await _db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return eventEntity is null
                ? ServiceResult<EventDto>.NotFound($"No existe el evento {id}.")
                : ServiceResult<EventDto>.Ok(ADto(eventEntity));
        }

        public async Task<ServiceResult<List<EventDto>>> GetByCharacterAsync(int characterId)
        {
            if (!await _db.Characters.AnyAsync(c => c.Id == characterId))
                return ServiceResult<List<EventDto>>.NotFound($"No existe el personaje {characterId}.");

            var events = await _db.Events.AsNoTracking().OrderBy(e => e.Year).ThenBy(e => e.Id).ToListAsync();
            return ServiceResult<List<EventDto>>.Ok(
                events.Where(e => e.ParticipantsIds.Contains(characterId)).Select(ADto).ToList());
        }

        public async Task<ServiceResult<EventDto>> CreateAsync(EventCreateDto dto)
        {
            var error = await ValidateAsync(dto, null);
            if (error is not null) return ServiceResult<EventDto>.Invalid(error);

            var eventEntity = new Event
            {
                Name = dto.Name.Trim(),
                Year = dto.Year,
                Location = dto.Location.Trim(),
                Description = dto.Description.Trim(),
                ParticipantsIds = dto.ParticipantsIds.Distinct().ToList(),
                DeadIds = new List<int>()
            };

            eventEntity.HashAutorizationCode =
                _hasher.HashPassword(eventEntity, dto.AutorizationCode);

            _db.Events.Add(eventEntity);
            await _db.SaveChangesAsync();

            return ServiceResult<EventDto>.Ok(ADto(eventEntity));
        }

        public async Task<ServiceResult<EventDto>> UpdateAsync(int id, EventCreateDto dto)
        {
            var eventEntity = await _db.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventEntity is null)
                return ServiceResult<EventDto>.NotFound($"No existe el evento {id}.");

            if (string.IsNullOrWhiteSpace(dto.AutorizationCode))
                return ServiceResult<EventDto>.Invalid("El código de autorización es obligatorio.");

            var verification = _hasher.VerifyHashedPassword(
                eventEntity, eventEntity.HashAutorizationCode, dto.AutorizationCode);
            if (verification == PasswordVerificationResult.Failed)
                return ServiceResult<EventDto>.Forbidden("El código de autorización es incorrecto.");

            if (!string.IsNullOrEmpty(eventEntity.Winner))
            {
                return ServiceResult<EventDto>.Invalid(
                    "No se puede modificar un evento que ya fue simulado.");
            }

            var error = await ValidateAsync(dto, eventEntity);
            if (error is not null) return ServiceResult<EventDto>.Invalid(error);

            eventEntity.Name = dto.Name.Trim();
            eventEntity.Year = dto.Year;
            eventEntity.Location = dto.Location.Trim();
            eventEntity.Description = dto.Description.Trim();
            eventEntity.ParticipantsIds = dto.ParticipantsIds.Distinct().ToList();
            eventEntity.HashAutorizationCode = _hasher.HashPassword(eventEntity, dto.AutorizationCode);

            await _db.SaveChangesAsync();

            return ServiceResult<EventDto>.Ok(ADto(eventEntity));
        }

        public async Task<ServiceResult<MvpResponse>> GetMvpAsync(int eventId)
        {
            var eventEntity = await _db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity is null)
                return ServiceResult<MvpResponse>.NotFound($"No existe el evento {eventId}.");

            var participants = await _db.Characters.AsNoTracking()
                .Include(c => c.Cards)
                .Where(c => eventEntity.ParticipantsIds.Contains(c.Id))
                .ToListAsync();

            MvpResponse? best = null;
            foreach (var character in participants)
            {
                var card = character.Cards
                    .Where(c => c.IsAvailable)
                    .OrderByDescending(c => c.DangerousLevel)
                    .ThenBy(c => c.Id)
                    .FirstOrDefault();

                if (card is null) continue;
                var score = PowerCalculator.Score(character, card);
                if (best is null || score > best.Score ||
                    (score == best.Score && character.Id < best.CharacterId))
                {
                    best = new MvpResponse(eventEntity.Id, character.Id, character.Name, card.Id,
                        card.DangerousLevel, PowerCalculator.Bonus(character), score);
                }
            }

            return best is null
                ? ServiceResult<MvpResponse>.Invalid("Ningún participante del evento tiene una carta.")
                : ServiceResult<MvpResponse>.Ok(best);
        }

        private async Task<string?> ValidateAsync(EventCreateDto dto, Event? current)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return "El nombre es obligatorio.";

            if (string.IsNullOrWhiteSpace(dto.Location))
                return "La ubicación es obligatoria.";

            if (string.IsNullOrWhiteSpace(dto.Description))
                return "La descripción es obligatoria.";

            if (string.IsNullOrWhiteSpace(dto.AutorizationCode) ||
                dto.AutorizationCode.Length < 8)
                return "El código de autorización debe tener al menos 8 caracteres.";

            var participants = dto.ParticipantsIds ?? new List<int>();

            if (participants.Count == 0)
                return "El evento necesita al menos un participante.";

            if (participants.Distinct().Count() != participants.Count)
                return "Hay participantes repetidos.";

            // Verificar que todos los participantes existan
            var characterIds = await _db.Characters.AsNoTracking()
                .Where(c => participants.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var characterId in participants)
            {
                if (!characterIds.Contains(characterId))
                    return $"No existe el personaje {characterId}.";
            }

            // Obtener eventos anteriores, excluyendo el evento actual si se está editando
            int? currentId = current?.Id;

            var previousEvents = await _db.Events.AsNoTracking()
                .Where(e => (currentId == null || e.Id != currentId.Value)
                            && e.Year <= dto.Year)
                .OrderBy(e => e.Year)
                .ToListAsync();

            // Comprobar que ningún participante haya muerto anteriormente
            foreach (var id in participants)
            {
                var deathEvent = previousEvents
                    .FirstOrDefault(e => e.DeadIds.Contains(id));

                if (deathEvent is not null)
                {
                    return $"{await NameOfAsync(id)} murió en '{deathEvent.Name}' " +
                           $"({YearConverter.Format(deathEvent.Year)}) " +
                           "y no puede participar en este evento.";
                }
            }

            return null;
        }

        private async Task<string> NameOfAsync(int id)
        {
            return await _db.Characters.AsNoTracking()
                .Where(c => c.Id == id).Select(c => c.Name).FirstOrDefaultAsync()
                ?? $"Personaje {id}";
        }

        private EventDto ADto (Event e)
        {
            return new EventDto
            {
            Id = e.Id,
            Name = e.Name,
            Year = e.Year,
            Location = e.Location,
            Description = e.Description,
            ParticipantsIds = e.ParticipantsIds,
            DeadIds = e.DeadIds,
            Winner = e.Winner is null ? null : _encryption.Decrypt(e.Winner)
            };
        }

    }
}