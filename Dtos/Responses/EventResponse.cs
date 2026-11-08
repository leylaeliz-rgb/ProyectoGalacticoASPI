namespace ProyectoGalactico.Dtos
{
    public record EventResponse(int Id, string Name, string Date, int Year, string Location,
                                 string Description, List<int> ParticipantIds,
                                 List<int> DeadIds, string? Winner);

    public record MvpResponse(int EventId, int CharacterId, string Name, int CardId,
                              int DangerousLevel, int SensitiveBonus, int Score);
}