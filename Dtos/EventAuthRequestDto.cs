using System.ComponentModel.DataAnnotations;
namespace ProyectoGalactico.Dtos
{
    public class EventAuthRequestDto
    {
        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código de autorización es obligatorio.")]
        public string AutorizationCode { get; set; } = string.Empty;
    }
}
