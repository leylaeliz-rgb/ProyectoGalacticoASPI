using ProyectoGalactico.Models;
namespace ProyectoGalactico.Services
{
    public static class PowerCalculator
    {
        public const int SensitiveBonus = 2;

        public static int Bonus(Character c) => c.fuerzaSensitivo ? SensitiveBonus : 0;

        // Poder = peligrosidad de la carta + bono si es sensible a la Fuerza
        public static int Score(Character c, CardCharacter card) => card.dangerousLevel + Bonus(c);

    }
}
