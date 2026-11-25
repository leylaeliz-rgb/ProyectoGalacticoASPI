namespace ProyectoGalactico.Dtos.Responses
{
    public record MvpResponse(int EventId, int CharacterId, string Name, int CardId,
                              int DangerousLevel, int SensitiveBonus, int Score);
}
