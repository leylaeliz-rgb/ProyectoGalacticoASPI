using System.ComponentModel.DataAnnotations;

using ProyectoGalactico.Enums;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Dtos
{
    public class CharacterCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(100)]
        public string Species { get; set; } = string.Empty;

        [Required(ErrorMessage = "La facción a la que pertenece es obligatoria")]
        public Faction? Faction { get; set; }
        public bool SensoryForce { get; set; }
    }
}
