using System.ComponentModel.DataAnnotations;
namespace ProyectoGalactico.Dtos;

public class EventCreateDto
{
    [Required(ErrorMessage = "El código de autorización es obligatorio.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "El código debe tener entre 8 y 100 caracteres.")]
    public string AutorizationCode { get; set; } = string.Empty;


    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;


    [Range(-999999, 999999, ErrorMessage = "El año está fuera del rango permitido.")]
    public int Year { get; set; }


    [Required(ErrorMessage = "La ubicación es obligatoria.")]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;


    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300)]
    public string Description { get; set; } = string.Empty;


    [MinLength(1, ErrorMessage = "Debe haber al menos un participante.")]
    public List<int> ParticipantsIds { get; set; } = new();
}
