using ProyectoGalactico.Enums;

namespace ProyectoGalactico.Dtos.Responses;

public record BattleHistoryItem(
    int BattleRecordId,
    int EventId,
    string EventName,
    int EventYear,
    string EventLocation,
    string? EventWinner,
    int? CardId,
    BattleOutcome Outcome,
    int? Roll,
    int? SurvivalRatio,
    bool Reappeared
);