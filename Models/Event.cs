namespace ProyectoGalactico.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> ParticipantsIds { get; set; } = new List<int>();
        public List<int> DeadIds { get; set; } = new List<int>();
        public string? Winner { get; set; }
    }
}