using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;
using Microsoft.AspNetCore.Identity;
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

        public async Task <IEnumerable<EventDto>> ListAsync()
        {

            var eventos = await _db.Events.AsNoTracking().OrderBy(e => e.Id).ToListAsync();
            return eventos.Select(ADto).ToList();
        }


        public async Task <EventDto?> GetByIdAsync(int id)
        {
            var eventVar = await _db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return eventVar is null ? null : ADto(eventVar);
        }
        public async Task <EventDto?> CreateAsync (EventCreateDto dto)
        {
            var eventVar = new Event
            {
                Name = dto.Name,
                Year = dto.Year,
                Location = dto.Location,
                Description = dto.Description,
                ParticipantsIds = dto.ParticipantsIds,
            };

            eventVar.HashAutorizationCode = _hasher.HashPassword(eventVar, dto.AutorizationCode);
            if (!string.IsNullOrWhiteSpace(dto.Winner))
            {
                eventVar.Winner = _encryption.Encrypt(dto.Winner);
            }
            _db.Events.Add(eventVar);
            await _db.SaveChangesAsync();
            return ADto(eventVar);
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
 
        //public ServiceResult<List<EventResponse>> GetAll(int? characterId, string? desde, string? hasta)
        //{
        //    int desdeYear = 0, hastaYear = 0;
        //    if (desde is not null && !YearConverter.TryParse(desde, out desdeYear))
        //        return ServiceResult<List<EventResponse>>.Invalid("'desde' no es una fecha válida (ej. 10 BBY, 3 ABY).");
        //    if (hasta is not null && !YearConverter.TryParse(hasta, out hastaYear))
        //        return ServiceResult<List<EventResponse>>.Invalid("'hasta' no es una fecha válida (ej. 10 BBY, 3 ABY).");

        //    lock (DataForModels.Sync)
        //    {
        //        var query = DataForModels.Events.AsEnumerable();
        //        if (characterId is not null) query = query.Where(e => e.participantIds.Contains(characterId.Value));
        //        if (desde is not null) query = query.Where(e => e.year >= desdeYear);
        //        if (hasta is not null) query = query.Where(e => e.year <= hastaYear);

        //        // Orden cronológico: por eso se guarda el año como número
        //        var list = query.OrderBy(e => e.year).ThenBy(e => e.Id).Select(ToResponse).ToList();
        //        return ServiceResult<List<EventResponse>>.Ok(list);
        //    }
        //}

        //public ServiceResult<EventResponse> GetById(int id)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var ev = DataForModels.Events.FirstOrDefault(e => e.Id == id);
        //        return ev is null
        //            ? ServiceResult<EventResponse>.NotFound($"No existe el evento {id}.")
        //            : ServiceResult<EventResponse>.Ok(ToResponse(ev));
        //    }
        //}

        //public ServiceResult<List<EventResponse>> GetByCharacter(int characterId)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        if (!DataForModels.Characters.Any(c => c.Id == characterId))
        //            return ServiceResult<List<EventResponse>>.NotFound($"No existe el personaje {characterId}.");

        //        var list = DataForModels.Events
        //            .Where(e => e.participantIds.Contains(characterId))
        //            .OrderBy(e => e.year).ThenBy(e => e.Id)
        //            .Select(ToResponse).ToList();

        //        return ServiceResult<List<EventResponse>>.Ok(list);
        //    }
        //}

        //public ServiceResult<EventResponse> Create(EventInput input)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var error = Validate(input, null, out var year);
        //        if (error is not null) return ServiceResult<EventResponse>.Invalid(error);

        //        var id = DataForModels.Events.Count == 0 ? 1 : DataForModels.Events.Max(e => e.Id) + 1;

        //        var created = new Event(id, input.name.Trim(), year, input.location.Trim(),
        //            input.description.Trim(), input.participantIds.ToList(), input.deadIds.ToList(), null);

        //        DataForModels.Events.Add(created);
        //        ApplyDeaths(created.deadIds); // efecto sobre la otra colección

        //        return ServiceResult<EventResponse>.Ok(ToResponse(created));
        //    }
        //}

        //public ServiceResult<EventResponse> Update(int id, EventInput input)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var index = DataForModels.Events.FindIndex(e => e.Id == id);
        //        if (index < 0) return ServiceResult<EventResponse>.NotFound($"No existe el evento {id}.");

        //        var current = DataForModels.Events[index];
        //        var error = Validate(input, current, out var year);
        //        if (error is not null) return ServiceResult<EventResponse>.Invalid(error);

        //        // Si cambian los participantes, el ganador ya no es válido
        //        var sameParticipants = current.participantIds.Order().SequenceEqual(input.participantIds.Order());

        //        var updated = current with
        //        {
        //            name = input.name.Trim(),
        //            year = year,
        //            location = input.location.Trim(),
        //            description = input.description.Trim(),
        //            participantIds = input.participantIds.ToList(),
        //            deadIds = input.deadIds.ToList(),
        //            winner = sameParticipants ? current.winner : null
        //        };

        //        DataForModels.Events[index] = updated;
        //        ApplyDeaths(updated.deadIds);

        //        return ServiceResult<EventResponse>.Ok(ToResponse(updated));
        //    }
        //}

        //public ServiceResult<MvpResponse> GetMvp(int eventId)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var ev = DataForModels.Events.FirstOrDefault(e => e.Id == eventId);
        //        if (ev is null) return ServiceResult<MvpResponse>.NotFound($"No existe el evento {eventId}.");

        //        MvpResponse? best = null;
        //        foreach (var pid in ev.participantIds)
        //        {
        //            var character = DataForModels.Characters.FirstOrDefault(c => c.Id == pid);
        //            var card = cards.GetBestCard(pid);
        //            if (character is null || card is null) continue;

        //            var score = PowerCalculator.Score(character, card);

        //            // En empate gana el de menor id, para que el resultado sea siempre el mismo
        //            if (best is null || score > best.Score || (score == best.Score && character.Id < best.CharacterId))
        //                best = new MvpResponse(ev.Id, character.Id, character.Name, card.Id,
        //                    card.dangerousLevel, PowerCalculator.Bonus(character), score);
        //        }

        //        return best is null
        //            ? ServiceResult<MvpResponse>.Invalid("Ningún participante del evento tiene carta.")
        //            : ServiceResult<MvpResponse>.Ok(best);
        //    }
        //}

        //// current es null al crear; al modificar es el evento actual (se excluye de las comparaciones)
        //private static string? Validate(EventInput i, Event? current, out int year)
        //{
        //    year = 0;
        //    if (string.IsNullOrWhiteSpace(i.name)) return "El nombre es obligatorio.";
        //    if (string.IsNullOrWhiteSpace(i.location)) return "La ubicación es obligatoria.";
        //    if (string.IsNullOrWhiteSpace(i.description)) return "La descripción es obligatoria.";
        //    if (!YearConverter.TryParse(i.year, out year))
        //        return "Fecha inválida. Usa formatos como '10 BBY', '3 ABY' o 'Batalla de Yavin'.";

        //    var newYear = year; // un parámetro out no se puede usar dentro de lambdas
        //    var participants = i.participantIds ?? [];
        //    var dead = i.deadIds ?? [];

        //    if (participants.Count == 0) return "El evento necesita al menos un participante.";
        //    if (participants.Distinct().Count() != participants.Count) return "Hay participantes repetidos.";
        //    if (dead.Distinct().Count() != dead.Count) return "Hay muertes repetidas.";

        //    foreach (var id in participants)
        //        if (!DataForModels.Characters.Any(c => c.Id == id))
        //            return $"No existe el personaje {id}.";

        //    if (dead.Any(id => !participants.Contains(id)))
        //        return "Los personajes que mueren deben estar entre los participantes.";

        //    var others = DataForModels.Events.Where(e => current is null || e.Id != current.Id).ToList();

        //    // 1. No se puede quitar una muerte ya registrada
        //    if (current is not null)
        //    {
        //        foreach (var id in current.deadIds)
        //            if (!dead.Contains(id))
        //                return $"No se puede quitar la muerte registrada de {NameOf(id)}.";
        //    }

        //    // 2. Nadie muere dos veces
        //    foreach (var id in dead)
        //    {
        //        var previous = others.FirstOrDefault(e => e.deadIds.Contains(id));
        //        if (previous is not null)
        //            return $"{NameOf(id)} ya murió en '{previous.name}'.";
        //    }

        //    // 3. Nadie participa después de su muerte
        //    foreach (var id in participants)
        //    {
        //        var death = others.FirstOrDefault(e => e.deadIds.Contains(id));
        //        if (death is not null && newYear > death.year)
        //            return $"{NameOf(id)} murió en '{death.name}' ({YearConverter.Format(death.year)}) y no puede participar en un evento posterior.";
        //    }

        //    // 4. Una muerte nueva no puede dejar participaciones posteriores ya registradas
        //    foreach (var id in dead)
        //    {
        //        var later = others.FirstOrDefault(e => e.year > newYear && e.participantIds.Contains(id));
        //        if (later is not null)
        //            return $"{NameOf(id)} participa en '{later.name}', que es posterior a esta muerte.";
        //    }

        //    return null;
        //}

        //// Un record no se modifica: se reemplaza por una copia con el estado cambiado
        //private static void ApplyDeaths(List<int> deadIds)
        //{
        //    foreach (var id in deadIds)
        //    {
        //        var index = DataForModels.Characters.FindIndex(c => c.Id == id);
        //        if (index >= 0)
        //            DataForModels.Characters[index] = DataForModels.Characters[index] with { estado = 'M' };
        //    }
        //}

        //private static string NameOf(int id) =>
        //    DataForModels.Characters.FirstOrDefault(c => c.Id == id)?.Name ?? $"Personaje {id}";

        //private static EventResponse ToResponse(Event e) =>
        //    new(e.Id, e.name, YearConverter.Format(e.year), e.year, e.location, e.description,
        //        e.participantIds, e.deadIds, e.winner);
    }
}