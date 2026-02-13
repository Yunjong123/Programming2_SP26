using System;

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
            return double.TryParse((input ?? "").Trim(), out _);
        }

        public static bool IsWholeNumber(string input)
        {
            return int.TryParse((input ?? "").Trim(), out _);
        }

        public static int ConvertStringToInteger(string input, int fallback = -1)
        {
            return int.TryParse((input ?? "").Trim(), out int value) ? value : fallback;
        }

        public static double ConvertStringToDouble(string input, double fallback = -1)
        {
            return double.TryParse((input ?? "").Trim(), out double value) ? value : fallback;
        }
    }
}