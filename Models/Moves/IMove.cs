using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Models.Moves;

public interface IMove
{
    string Name { get; }
    PokemonType Type { get; }
    int Priority { get; }
    void Execute(MoveContext ctx);
    IMove Clone();
}