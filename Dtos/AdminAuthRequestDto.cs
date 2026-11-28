using System.ComponentModel.DataAnnotations;

namespace ProyectoGalactico.Dtos;

public class AdminAuthRequestDto
{
    [Required(ErrorMessage = "El código de administrador es obligatorio.")]
    public string AutorizationCode { get; set; } = string.Empty;
}