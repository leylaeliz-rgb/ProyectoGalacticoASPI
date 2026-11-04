namespace ProyectoGalactico.Models
{
    //enum faccion
    public class Character
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public faccion 
        //}
        //public record Character(int Id, string Name, string species, char faccion,
        //                        string afiliacion, char estado, bool fuerzaSensitivo);
        //public record CharacterInput(string name, string species, char faccion,
        //                        string afiliacion, char estado, bool fuerzaSensitivo);
    }
}

//namespace ProyectoGalactico.Models
//{
//    public class Event
//    {
//        public int Id { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public int Year { get; set; }
//        public string Location { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;
//        public List<int> ParticipantsIds { get; set; } = new List<int>();
//        public List<int> DeadIds { get; set; } = new List<int>();
//        public string? Winner { get; set; }
//    }
//}