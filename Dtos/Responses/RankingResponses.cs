namespace ProyectoGalactico.Dtos.Responses
{
    public class RankingResponses
    {
        public record RankingItem(int Position, int CharacterId, string Name, string Faction, int CardId, int DangerousLevel,
                              int SensitiveBonus, int Score);
    }
}
