using PokemonCliBattleManager.Models.Moves;

namespace PokemonCliBattleManager.Models.Pokemon;

public sealed class SpeciesPokemon : Pokemon
{
    public SpeciesPokemon(
        string name,
        PokemonType primaryType,
        PokemonType? secondaryType,
        int maxHp,
        int attack,
        int defense,
        int spAttack,
        int spDefense,
        int speed,
        IEnumerable<IMove> moves)
        : base(name, primaryType, secondaryType, maxHp, attack, defense, spAttack, spDefense, speed, moves)
    {
    }

    public SpeciesPokemon CloneFresh()
    {
        return new SpeciesPokemon(
            name: Name,
            primaryType: PrimaryType,
            secondaryType: SecondaryType,
            maxHp: MaxHp,
            attack: Attack,
            defense: Defense,
            spAttack: SpAttack,
            spDefense: SpDefense,
            speed: Speed,
            moves: Moves.Select(m => m.Clone())
        );
    }
}