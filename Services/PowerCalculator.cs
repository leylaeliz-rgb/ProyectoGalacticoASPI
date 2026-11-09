using ProyectoGalactico.Models;
namespace ProyectoGalactico.Services;

public static class PowerCalculator
{
    public const int SensitiveBonus = 2;
    public static int Bonus(Character character) => character.SensoryForce ? SensitiveBonus : 0;
    public static int Score(Character character, CardCharacter card) => card.DangerousLevel + Bonus(character);
}
