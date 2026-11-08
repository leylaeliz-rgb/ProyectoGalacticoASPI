namespace ProyectoGalactico.Dtos.Responses
{
    public class RankingResponses
    {
        public record RankingItem(int Position, int CharacterId, string Name, string Faction,
                              string Affiliation, int CardId, int DangerousLevel,
                              int SensitiveBonus, int Score);
    }
}
