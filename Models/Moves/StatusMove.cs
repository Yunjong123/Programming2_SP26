using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Models.Moves;

public sealed class StatusMove : IMove
{
    public string Name { get; }
    public PokemonType Type { get; }
    public int Accuracy { get; }
    public StatusCondition StatusToApply { get; }
    public int Priority { get; }

    public StatusMove(string name, PokemonType type, int accuracy, StatusCondition statusToApply, int priority = 0)
    {
        Name = name;
        Type = type;
        Accuracy = Math.Clamp(accuracy, 1, 100);
        StatusToApply = statusToApply;
        Priority = priority;
    }

    public void Execute(MoveContext ctx)
    {
        if (ctx.Target.IsFainted) return;

        if (ctx.Target.Status.Condition != StatusCondition.None)
        {
            ctx.Battle.Ui.WriteLine($"{ctx.Target.Name} is already affected by a status condition.");
            return;
        }

        if (!ctx.Target.CanReceiveStatus(StatusToApply))
        {
            ctx.Battle.Ui.WriteLine($"{ctx.Target.Name} is immune to {StatusToApply}.");
            return;
        }

        if (ctx.Target.TryApplyStatus(StatusToApply, ctx.Battle.Rng, out var msg))
        {
            ctx.Battle.Ui.WriteLine(msg);
        }
        else
        {
            ctx.Battle.Ui.WriteLine("Nothing happened.");
        }
    }

    public IMove Clone()
    {
        return new StatusMove(Name, Type, Accuracy, StatusToApply, Priority);
    }
}