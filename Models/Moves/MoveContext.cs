using PokemonCliBattleManager.Battle;
using PokemonCliBattleManager.Core;
using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Models.Moves;

public sealed class MoveContext
{
    public BattleContext Battle { get; }
    public Trainer UserTrainer { get; }
    public Trainer TargetTrainer { get; }
    public SpeciesPokemon User { get; }
    public SpeciesPokemon Target { get; }
    public SideState UserSide { get; }
    public SideState TargetSide { get; }

    public MoveContext(
        BattleContext battle,
        Trainer userTrainer,
        Trainer targetTrainer,
        SpeciesPokemon user,
        SpeciesPokemon target,
        SideState userSide,
        SideState targetSide)
    {
        Battle = battle;
        UserTrainer = userTrainer;
        TargetTrainer = targetTrainer;
        User = user;
        Target = target;
        UserSide = userSide;
        TargetSide = targetSide;
    }
}