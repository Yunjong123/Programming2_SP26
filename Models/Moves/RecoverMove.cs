using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Models.Moves;

public sealed class RecoverMove : IMove
{
    public string Name { get; }
    public PokemonType Type { get; }
    public int HealAmount { get; }
    public bool CuresStatus { get; }
    public int Priority { get; }
    public RecoverMove(string name, PokemonType type, int healAmount, bool curesStatus = false, int priority = 0)
    {
        Name = name;
        Type = type;
        HealAmount = Math.Max(1, healAmount);
        CuresStatus = curesStatus;
        Priority = priority;
    }

    public void Execute(MoveContext ctx)
    {
        if (ctx.User.IsFainted) return;

        var before = ctx.User.CurrentHp;
        ctx.User.Heal(HealAmount);
        var healed = ctx.User.CurrentHp - before;

        if (CuresStatus && ctx.User.Status.Condition != StatusCondition.None)
        {
            ctx.User.ClearStatus();
            ctx.Battle.Ui.WriteLine($"{ctx.User.Name} healed {healed} HP and cured its status.");
        }
        else
        {
            ctx.Battle.Ui.WriteLine($"{ctx.User.Name} restored {healed} HP.");
        }
    }

    public IMove Clone()
    {
        return new RecoverMove(Name, Type, HealAmount, CuresStatus, Priority);
    }
}