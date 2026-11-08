using ProyectoGalactico.Common;
using ProyectoGalactico.Data;
using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ProyectoGalactico.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly AppDbContext _db;
        // Las listas son estáticas y compartidas: el lock evita choques entre peticiones
        //public ServiceResult<List<Character>> GetAll(string? faccion, bool? fuerzaSensitivo)
        //{
        //    char? code = null;
        //    if (faccion is not null)
        //    {
        //        code = GameCodes.ParseFaction(faccion);
        //        if (code is null)
        //            return ServiceResult<List<Character>>.Invalid("Facción no válida. Usa Rebelde, Imperio o Neutral.");
        //    }

        //    lock (DataForModels.Sync)
        //    {
        //        var query = DataForModels.Characters.AsEnumerable();
        //        if (code is not null) query = query.Where(c => c.faccion == code);
        //        if (fuerzaSensitivo is not null) query = query.Where(c => c.fuerzaSensitivo == fuerzaSensitivo);
        //        return ServiceResult<List<Character>>.Ok(query.ToList());
        //    }
        //}

        //public ServiceResult<Character> GetById(int id)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var character = DataForModels.Characters.FirstOrDefault(c => c.Id == id);
        //        return character is null
        //            ? ServiceResult<Character>.NotFound($"No existe el personaje {id}.")
        //            : ServiceResult<Character>.Ok(character);
        //    }
        //}

        //public ServiceResult<Character> Create(CharacterInput input)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var error = Validate(input, null);
        //        if (error is not null) return ServiceResult<Character>.Invalid(error);
        //        var id = DataForModels.Characters.Count == 0
        //            ? 1
        //            : DataForModels.Characters.Max(c => c.Id) + 1;

        //        var created = new Character(id, input.name.Trim(), input.species.Trim(),
        //            input.faccion, input.afiliacion, input.estado, input.fuerzaSensitivo);

        //        DataForModels.Characters.Add(created);
        //        return ServiceResult<Character>.Ok(created);
        //    }
        //}

        //public ServiceResult<Character> Update(int id, CharacterInput input)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var index = DataForModels.Characters.FindIndex(c => c.Id == id);
        //        if (index < 0) return ServiceResult<Character>.NotFound($"No existe el personaje {id}.");

        //        var error = Validate(input, id);
        //        if (error is not null) return ServiceResult<Character>.Invalid(error);

        //        // Un record no se modifica: se crea una copia con 'with' y se reemplaza en la lista
        //        var updated = DataForModels.Characters[index] with
        //        {
        //            Name = input.name.Trim(),
        //            species = input.species.Trim(),
        //            faccion = input.faccion,
        //            afiliacion = input.afiliacion,
        //            estado = input.estado,
        //            fuerzaSensitivo = input.fuerzaSensitivo
        //        };

        //        DataForModels.Characters[index] = updated;
        //        return ServiceResult<Character>.Ok(updated);
        //    }
        //}

        //public ServiceResult<bool> Delete(int id)
        //{
        //    lock (DataForModels.Sync)
        //    {
        //        var character = DataForModels.Characters.FirstOrDefault(c => c.Id == id);
        //        if (character is null) return ServiceResult<bool>.NotFound($"No existe el personaje {id}.");

        //        if (DataForModels.Events.Any(e => e.participantIds.Contains(id)))
        //            return ServiceResult<bool>.Invalid("No se puede eliminar: el personaje participa en eventos.");

        //        DataForModels.Cards.RemoveAll(c => c.characterId == id); // borrado en cascada
        //        DataForModels.Characters.Remove(character);
        //        return ServiceResult<bool>.Ok(true);
        //    }
        //}
        //private static string? Validate(CharacterInput i, int? id)
        //{
        //    if (string.IsNullOrWhiteSpace(i.name)) return "El nombre es obligatorio.";
        //    if (string.IsNullOrWhiteSpace(i.species)) return "La especie es obligatoria.";
        //    if (!GameCodes.IsValidFaction(i.faccion)) return "La facción debe ser R (Rebelde), I (Imperio) o N (Neutral).";
        //    if (!GameCodes.IsValidState(i.estado)) return "El estado debe ser V (vivo), M (muerto) o D (desconocido).";

        //    var hasRegisteredDeath = id is not null && DataForModels.Events.Any(e => e.deadIds.Contains(id.Value));

        //    if (i.estado == 'M' && !hasRegisteredDeath)
        //        return "Un personaje solo pasa a muerto cuando un evento registra su muerte.";
        //    if (hasRegisteredDeath && i.estado != 'M')
        //        return "Este personaje tiene una muerte registrada en un evento: su estado debe seguir siendo M.";

        //    return null;
        //}
    }
}