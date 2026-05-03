using PokemonCliBattleManager.Models.Pokemon;
using PokemonCliBattleManager.Rules;

namespace PokemonCliBattleManager.Models.Moves;

public sealed class DamageMove : IMove
{
    public string Name { get; }
    public PokemonType Type { get; }
    public int Power { get; }
    public int Accuracy { get; }
    public int Priority { get; }
    public MoveCategory Category { get; }

    public int StatusChance { get; } // 0..100
    public StatusCondition StatusOnHit { get; }

    public DamageMove(
        string name,
        PokemonType type,
        int power,
        int accuracy,
        MoveCategory category,
        int priority = 0,
        int statusChance = 0,
        StatusCondition statusOnHit = StatusCondition.None)
    {
        Name = name;
        Type = type;
        Power = Math.Max(1, power);
        Accuracy = Math.Clamp(accuracy, 1, 100);
        Category = category;
        Priority = priority;

        StatusChance = Math.Clamp(statusChance, 0, 100);
        StatusOnHit = statusOnHit;
    }

    public void Execute(MoveContext ctx)
    {
        if (ctx.Target.IsFainted) return;

        var roll = ctx.Battle.Rng.Next(1, 101);
        if (roll > Accuracy)
        {
            ctx.Battle.Ui.WriteLine("The attack missed!");
            return;
        }

        var stab = (ctx.User.HasType(Type)) ? 1.5 : 1.0; //Gen 4 STAB
        var eff = TypeChart.Effectiveness(Type, ctx.Target.PrimaryType, ctx.Target.SecondaryType);
        var rand = 0.85 + ctx.Battle.Rng.NextDouble() * 0.15;

        var atk = ctx.User.GetEffectiveOffense(Category);
        var def = ctx.User.GetEffectiveDefense(Category);

        var baseDamage = Power * (atk / (double)Math.Max(1, def));
        var dmg = (int)Math.Floor(baseDamage * stab * eff * rand);
        dmg = Math.Max(1, dmg);

        ctx.Target.TakeDamage(dmg);

        if (eff >= 1.0) ctx.Battle.Ui.WriteLine("It's super effective!");
        else if (eff <= 0.1) ctx.Battle.Ui.WriteLine("It doesn't affect the target...");
        else if (eff <= 0.6) ctx.Battle.Ui.WriteLine("It's not very effective...");

        ctx.Battle.Ui.WriteLine($"{ctx.Target.Name} took {dmg} damage.");

        TryApplyOnHitStatus(ctx);
    }

    private void TryApplyOnHitStatus(MoveContext ctx)
    {
        if (StatusOnHit == StatusCondition.None) return;
        if (ctx.Target.IsFainted) return;
        if (ctx.Target.Status.Condition != StatusCondition.None) return;

        var roll = ctx.Battle.Rng.Next(1, 101);
        if (roll > StatusChance) return;

        if (ctx.Target.TryApplyStatus(StatusOnHit, ctx.Battle.Rng, out var msg))
        {
            ctx.Battle.Ui.WriteLine(msg);
        }
    }

    public IMove Clone()
    {
        return new DamageMove(Name, Type, Power, Accuracy, Category, Priority, StatusChance, StatusOnHit);
    }
}

