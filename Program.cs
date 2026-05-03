using PokemonCliBattleManager.Core;

namespace PokemonCliBattleManager;

internal static class Program
{
    private static void Main()
    {
        var ui = new ConsoleUi();
        var game = new Game(ui);
        game.Run();
    }
}