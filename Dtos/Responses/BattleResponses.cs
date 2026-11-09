using ProyectoGalactico.Enums;

namespace ProyectoGalactico.Dtos.Responses
{
    public record FighterScore(int CharacterId, string Name, int CardId,
                               int DangerousLevel, int SensitiveBonus, int Score);

    public record SideResult(string Faction, List<FighterScore> Members, double BaseTotal,
                             double Support, double Factor, double FinalTotal);

    public record BattleResult(int EventId, string EventName, SideResult Autobot, SideResult Decepticon,
                               List<FighterScore> Neutrals, string SupportNote, string Winner,
                               string Criteria, int Seed);
    public record BattleConsequence(
    int CharacterId,
    string CharacterName,
    int CardId,
    int? Roll,
    int SurvivalRatio,
    BattleOutcome Outcome
);
}
