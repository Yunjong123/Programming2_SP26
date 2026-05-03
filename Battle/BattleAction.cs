using PokemonCliBattleManager.Items;

namespace PokemonCliBattleManager.Battle;

public enum ActionKind
{
    Attack = 1,
    Switch = 2,
    UseItem = 3
}

public sealed record BattleAction(
    ActionKind Kind,
    int Priority,
    int Speed,
    int? MoveIndex = null,
    int? SwitchIndex = null,
    IItem? Item = null
);
