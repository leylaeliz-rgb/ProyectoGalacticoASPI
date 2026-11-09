using ProyectoGalactico.Enums;
using ProyectoGalactico.Models;
public class BattleRecord
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
    public int? CardId { get; set; }              // null si no apareció o si se borró la carta
    public CardCharacter? Card { get; set; }
    public BattleOutcome Outcome { get; set; }
    public int? Roll { get; set; }                // 1 a 100
    public int? SurvivalRatio { get; set; }
    public bool Reappeared { get; set; }
}