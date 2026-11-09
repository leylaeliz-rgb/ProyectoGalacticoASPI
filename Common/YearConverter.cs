using System.Globalization;

namespace ProyectoGalactico.Common
{
    public static class YearConverter
    {
        public static bool TryParse(string? text, out int year)
        {
            year = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            var t = text.Trim().ToUpperInvariant();

            if (t == "CAIDA DE LOS PRIMES" ||
                t == "CAÍDA DE LOS PRIMES")
                return true;

            var parts = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], NumberStyles.None,
                    CultureInfo.InvariantCulture, out var n))
                return false;

            if (parts[1] == "DP")
            {
                year = n;
                return true;
            }

            if (parts[1] == "AP")
            {
                year = -n;
                return true;
            }

            return false;
        }

        public static string Format(int year) =>
            year == 0
                ? "Caída de los Primes"
                : year < 0
                    ? $"{-year} AP"
                    : $"{year} DP";
    }
}