using System.ComponentModel.DataAnnotations;
namespace ProyectoGalactico.Dtos;

public class CardCharacterCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El identificador del personaje debe ser válido.")]
    public int CharacterId { get; set; }
    [Required(ErrorMessage = "El poder es obligatorio.")]
    [StringLength(150)]
    public string Power { get; set; } = string.Empty;
    [Required(ErrorMessage = "La habilidad especial es obligatoria.")]
    [StringLength(200)]
    public string SpecialHability { get; set; } = string.Empty;
    [Required(ErrorMessage = "El arma es obligatoria.")]
    [StringLength(150)]
    public string Weapon { get; set; } = string.Empty;
    [Range(1, 10, ErrorMessage = "El nivel de peligrosidad debe estar entre 1 y 10.")]
    public int DangerousLevel { get; set; }
    [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
    [StringLength(500)]
    public string PictureUrl { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "El ratio de supervivencia tiene que ser entre 1 a 100")]
    public int SurvivalRatio { get; set; } = 50;       // 1 a 100
}
