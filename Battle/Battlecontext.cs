using PokemonCliBattleManager.Core;

namespace PokemonCliBattleManager.Battle;

public sealed class BattleContext
{
    public ConsoleUi Ui { get; }
    public Random Rng { get; }

    public BattleContext(ConsoleUi ui, Random rng)
    {
        Ui = ui;
        Rng = rng;
    }
}