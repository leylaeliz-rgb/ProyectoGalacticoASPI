using System.ComponentModel.DataAnnotations;

namespace ProyectoGalactico.Dtos
{
    public class EventCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio" )]
        [StringLength( 100 )]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(1, 999999, ErrorMessage = "El año debe ser un número positivo")]
        public int Year { get; set; }

        [Required(ErrorMessage = "La locación es obligatorio")]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatorio")]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe haber al menos un participante")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un participante")]
        public List<int> ParticipantsIds { get; set; } = new List<int>();

        public List<int> DeadIds { get; set; } = new List<int>();

        [StringLength(100)]
        public string? Winner { get; set; }
    }
}