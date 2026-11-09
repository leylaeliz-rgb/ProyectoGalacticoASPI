using ProyectoGalactico.Enums;

namespace ProyectoGalactico.Common;

public static class GameCodes
{
    public static bool TryParseFaction(string? value, out Faction faction)
    {
        if (Enum.TryParse(value, true, out faction) && Enum.IsDefined(faction))
            return true;

        faction = default;
        return false;
    }
}
