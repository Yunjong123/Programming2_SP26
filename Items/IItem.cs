using PokemonCliBattleManager.Battle;
using PokemonCliBattleManager.Core;

namespace PokemonCliBattleManager.Items;

public interface IItem
{
    string Name { get; }
    void Use(BattleContext ctx, Trainer user, Trainer opponent);
}