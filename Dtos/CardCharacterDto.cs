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
}
