namespace ProyectoGalactico.Dtos
{
    public class EventAuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string EventName { get; set; } = string.Empty;
    }
}
