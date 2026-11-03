namespace ProyectoGalactico.Models
{
    public record CardsCharacter(int Id, int characterId, string power, string SpecialHability
                        , string weapon, int dangerousLevel, string pictureUrl);
    public record CardsCharacterInput(int characterId, string power, string SpecialHability
                        , string weapon, int dangerousLevel, string pictureUrl);
}
