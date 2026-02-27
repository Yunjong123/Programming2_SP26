using System;
using System.Globalization;

namespace CraftingSystemMonday
{
    public static class Library
    {
        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static string GetInput(string prompt = null)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Print(prompt);
            }

            string input = Console.ReadLine();
            return (input ?? "").Trim();
        }

        public static bool IsNumber(string input)
        {
            return double.TryParse((input ?? "").Trim(), NumberStyles.Float, CultureInfo.InstalledUICulture, out _);
        }

        public static bool IsWholeNumber(string input)
        {
            return int.TryParse((input ?? "").Trim(), NumberStyles.Integer, CultureInfo.InstalledUICulture, out _);
        }

        public static int ConvertStringToInteger(string input, int fallback = -1)
        {
            return int.TryParse((input ?? "").Trim(), NumberStyles.Integer, CultureInfo.InstalledUICulture, out int value) ? value : fallback;
        }

        public static double ConvertStringToDouble(string input, double fallback = -1)
        {
            return double.TryParse((input ?? "").Trim(), NumberStyles.Float, CultureInfo.InstalledUICulture, out double value) ? value : fallback;
        }

        public static string ReadTextFileSafe(string path, string fallback = "")
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            catch { }
            return fallback ?? "";
        }
    }
}