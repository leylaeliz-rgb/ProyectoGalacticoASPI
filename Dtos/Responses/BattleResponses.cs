namespace ProyectoGalactico.Dtos.Responses
{
    public record FighterScore(int CharacterId, string Name, string Affiliation, int CardId,
                                int DangerousLevel, int SensitiveBonus, int Score);

    public record SideResult(string Faction, List<FighterScore> Members, double BaseTotal,
                             double Support, double Factor, double FinalTotal);

    public record BattleResult(int EventId, string EventName, SideResult Rebelde, SideResult Imperio,
                               List<FighterScore> Neutrals, string SupportNote, string Winner,
                               string Criteria, int Seed);

    public record RankingItem(int Position, int CharacterId, string Name, string Faction,
                              string Affiliation, int CardId, int DangerousLevel,
                              int SensitiveBonus, int Score);

}
