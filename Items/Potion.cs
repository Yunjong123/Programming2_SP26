using PokemonCliBattleManager.Battle;
using PokemonCliBattleManager.Core;

namespace PokemonCliBattleManager.Items;

public sealed class Potion : IItem
{
    public string Name => "Potion";

    public void Use(BattleContext ctx, Trainer user, Trainer opponent)
    {
        var p = user.Active;
        if (p.IsFainted)
        {
            ctx.Ui.WriteLine($"{user.Name} can't use {Name} on a fainted Pokemon.");
            return;
        }

        var before = p.CurrentHp;
        p.Heal(30);
        var healed = p.CurrentHp - before;

        ctx.Ui.WriteLine($"{user.Name} used {Name} on {p.Name}/ (+{healed} HP)");
    }
}