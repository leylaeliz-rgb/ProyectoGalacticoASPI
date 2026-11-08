using ProyectoGalactico.Enums;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Dtos
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public Faction Faction { get; set; }
        public CharacterState CharacterState { get; set; }
        public bool IsActive { get; set; }
        public bool SensoryForce { get; set; }
        public List<CardCharacter> Cards { get; set; } = new();
    }
}
