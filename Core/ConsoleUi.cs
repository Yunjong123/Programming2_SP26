using System.Globalization;

namespace PokemonCliBattleManager.Core;

public sealed class ConsoleUi
{
    public void Clear() => Console.Clear();
    public void WriteLine(string text = "") => Console.WriteLine(text);
    public void Write(string text) => Console.Write(text);
    public void Pause(string prompt = "Press Enter to continue.")
    {
        WriteLine();
        WriteLine(prompt);
        Console.ReadLine();
    }

    public string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Write($"{prompt} ");
            var s = Console.ReadLine()?.Trim() ?? "";
            if (s.Length > 0) return s;
            WriteLine("Empty input is not allowed.");
        }
    }

    public int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Write($"{prompt} ");
            var s = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            {
                if (v >= min && v <= max) return v;
            }
            WriteLine($"Enter afn integer in range {min} to {max}.");
        }
    }

    public bool Confirm(string prompt)
    {
        while (true)
        {
            Write($"{prompt} (y/n) ");
            var s = (Console.ReadLine()?.Trim() ?? "").ToLowerInvariant();
            if (s is "y" or "yes") return true;
            if (s is "n" or "no") return false;
            WriteLine("Enter 'y' or 'n'.");
        }
    }

}