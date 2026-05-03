using PokemonCliBattleManager.Battle;
using PokemonCliBattleManager.Core;

namespace PokemonCliBattleManager.Items;

public sealed class FullHeal : IItem
{
    public string Name => "Full Heal";
    public void Use(BattleContext ctx, Trainer user, Trainer oppenent)
    {
        var p = user.Active;
        if (p.IsFainted)
        {
            ctx.Ui.WriteLine($"{user.Name} can't use {Name} on a fainted Pokemon.");
            return;
        }

        if (p.Status.Condition == Models.Pokemon.StatusCondition.None)
        {
            ctx.Ui.WriteLine($"{p.Name} has no status condition.");
            return;
        }

        p.ClearStatus();
        ctx.Ui.WriteLine($"{user.Name} used {Name}. {p.Name} was cured!");
    }
}