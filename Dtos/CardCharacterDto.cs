namespace ProyectoGalactico.Dtos;

public class CardCharacterDto
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public string Power { get; set; } = string.Empty;
    public string SpecialHability { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public int DangerousLevel { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
    public int SurvivalRatio { get; set; }          // 1 a 100
    public bool IsAvailable { get; set; } = true;   // false = inutilizada
}
