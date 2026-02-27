using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CraftingSystemMonday
{
    public static class UnitConverter
    {
        private static readonly Dictionary<string, double> toTsp = new Dictionary<string, double>
        {
            { "tsp", 1.0 },
            { "teaspoon", 1.0 },
            { "teaspoons", 1.0 },

            { "tbsp", 3.0 },
            { "tablespoon", 3.0 },
            { "tablespoons", 3.0 },

            { "cup", 48.0 },
            { "cups", 48.0 },

            { "pint", 96.0 },
            { "pints", 96.0 }
        };

        private static string Key(string s)
        {
            return (s ?? "").Trim().ToLowerInvariant();
        }

        public static bool CanConvert(string fromUnit, string toUnit)
        {
            return toTsp.ContainsKey(Key(fromUnit)) && toTsp.ContainsKey(Key(toUnit));
        }

        public static double Convert(double amount, string fromUnit, string toUnit)
        {
            string f = Key(fromUnit);
            string t = Key(toUnit);

            if (!toTsp.ContainsKey(f) || !toTsp.ContainsKey(t)) return double.NaN;

            double tsp = amount * toTsp[f];
            return tsp / toTsp[t];
        }
    }
}