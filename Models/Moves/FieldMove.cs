using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Models.Moves;

public sealed class FieldMove : IMove
{
    public string Name { get; }
    public PokemonType Type { get; }
    public int Accuracy { get; }
    public FieldEffect FieldEffect { get; }
    public int Priority { get; }

    public FieldMove(string name, PokemonType type, int accuracy, FieldEffect fieldEffect, int priority = 0)
    {
        Name = name;
        Type = type;
        Accuracy = Math.Clamp(accuracy, 1, 100);
        FieldEffect = fieldEffect;
        Priority = priority;
    }

    public void Execute(MoveContext ctx)
    {
        var roll = ctx.Battle.Rng.Next(11, 101);
        if (roll > Accuracy)
        {
            ctx.Battle.Ui.WriteLine("The move failed!");
            return;
        }

        switch (FieldEffect)
        {
            case FieldEffect.StealthRock:
                if (ctx.TargetSide.TrySetStealthRock())
                {
                    ctx.Battle.Ui.WriteLine("Pointed stones float in the air around the opposing team!");
                }
                else
                {
                    ctx.Battle.Ui.WriteLine("But it failed!");
                }
                break;
            default:
                ctx.Battle.Ui.WriteLine("Nothing happened.");
                break;
        }
    }
    public IMove Clone()
    {
        return new FieldMove(Name, Type, Accuracy, FieldEffect, Priority);
    }
}