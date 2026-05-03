using PokemonCliBattleManager.Core;
using PokemonCliBattleManager.Items;
using PokemonCliBattleManager.Models.Moves;
using PokemonCliBattleManager.Models.Pokemon;
using PokemonCliBattleManager.Rules;

namespace PokemonCliBattleManager.Battle;

public enum BattleResult
{
    PlayerWin,
    PlayerLose,
    Quit
}

public sealed class BattleSystem
{
    private readonly BattleContext _ctx;

    private const int SwitchPriority = 6;
    private const int ItemPriority = 6;

    private readonly SideState _playerSide = new();
    private readonly SideState _enemySide = new();

    public BattleSystem(BattleContext ctx)
    {
        _ctx = ctx;
    }

    public BattleResult RunBattle(Trainer player, Trainer enemy)
    {
        EnsureValidActive(player);
        EnsureValidActive(enemy);

        ApplyEntryHazardsIfAny(player, _playerSide);
        ApplyEntryHazardsIfAny(enemy, _enemySide);

        while (true)
        {
            if (!player.HasAvailablePokemon()) return BattleResult.PlayerLose;
            if (!enemy.HasAvailablePokemon()) return BattleResult.PlayerWin;

            EnsureValidActive(player);
            EnsureValidActive(enemy);

            _ctx.Ui.Clear();
            PrintHeader(player, enemy);

            var playerAction = DecideHumanAction(player);
            var enemyAction = DecideAiAction(enemy, player, _enemySide, _playerSide);

            var order = OrderActions(player, enemy, playerAction, enemyAction);

            foreach (var (actor, target, action, actorSide, targetSide) in order)
            {
                if (!actor.HasAvailablePokemon() || !target.HasAvailablePokemon()) break;

                EnsureValidActive(player);
                EnsureValidActive(enemy);

                if (actor.Active.IsFainted) continue;

                ExecuteAction(actor, target, action, actorSide, targetSide);

                if (!player.HasAvailablePokemon()) return BattleResult.PlayerLose;
                if (!enemy.HasAvailablePokemon()) return BattleResult.PlayerWin;

                EnsureValidActive(player);
                EnsureValidActive(enemy);
            }

            ApplyEndOfTurnEffects(player, _playerSide);
            ApplyEndOfTurnEffects(enemy, _enemySide);

            if (!player.HasAvailablePokemon()) return BattleResult.PlayerLose;
            if (!enemy.HasAvailablePokemon()) return BattleResult.PlayerWin;

            EnsureValidActive(player);
            EnsureValidActive(enemy);

            _ctx.Ui.Pause();
        }
    }

    private void PrintHeader(Trainer player, Trainer enemy)
    {
        var p = player.Active;
        var e = enemy.Active;

        var pTypes = p.SecondaryType is null ? $"{p.PrimaryType}" : $"{p.PrimaryType}/{p.SecondaryType}";
        var eTypes = e.SecondaryType is null ? $"{e.PrimaryType}" : $"{e.PrimaryType}/{e.SecondaryType}";

        _ctx.Ui.WriteLine($"{player.Name} | Active: {p.Name} [{pTypes}] HP {p.CurrentHp}/{p.MaxHp} Status {p.Status}");
        _ctx.Ui.WriteLine($"{enemy.Name} | Active: {e.Name} [{eTypes}] HP {e.CurrentHp}/{e.MaxHp} Status {e.Status}");

        PrintEnemyPartyStatus(enemy);

        _ctx.Ui.WriteLine($"Field: Your side Rocks{(_playerSide.HasStealthRock ? "ON" : "OFF")} | Enemy side Rocks={(_enemySide.HasStealthRock ? "ON" : "OFF")}");
        _ctx.Ui.WriteLine();
    }

    private void PrintEnemyPartyStatus(Trainer enemy)
    {
        _ctx.Ui.WriteLine();
        _ctx.Ui.WriteLine("[Enemy Pokemon HP]");

        for (var i = 0; i < enemy.Party.Count; i++)
        {
            var p = enemy.Party[i];
            var marker = i == enemy.ActiveIndex ? "*" : " ";
            var hp = p.IsFainted ? "FANITED" : $"{p.CurrentHp}/{p.MaxHp}";

            _ctx.Ui.WriteLine($"{marker} {i + 1}. {p.Name} HP {hp} Status {p.Status}");
        }

        _ctx.Ui.WriteLine();
    }

    private void EnsureValidActive(Trainer t)
    {
        if (!t.HasAvailablePokemon()) return;
        if (!t.Active.IsFainted) return;

        var idx = t.FirstAvailableIndex();
        if (idx >= 0) t.TrySwitchTo(idx);
    }

    private BattleAction DecideHumanAction(Trainer player)
    {
        while (true)
        {
            _ctx.Ui.WriteLine("Choose an action.");
            _ctx.Ui.WriteLine("1) Attack 2) Switch 3 Use Item 4) View Party");
            var pick = _ctx.Ui.ReadIntInRange("Number:", 1, 4);

            if (pick == 4)
            {
                PrintParty(player);
                continue;
            }

            if (pick == 1)
            {
                var moveIndex = PickMove(player.Active);
                var move = player.Active.Moves[moveIndex];
                return new BattleAction(ActionKind.Attack, move.Priority, player.Active.GetEffectiveSpeed(), MoveIndex: moveIndex);
            }

            if (pick == 2)
            {
                if (player.SwitchableIndices().Count == 0)
                {
                    _ctx.Ui.WriteLine("No switchable Pokemon. Defaulting to Attack.");
                    var moveIndex = PickMove(player.Active);
                    var move = player.Active.Moves[moveIndex];
                    return new BattleAction(ActionKind.Attack, move.Priority, player.Active.GetEffectiveSpeed(), moveIndex);
                }

                var switchIndex = PickSwitchIndex(player);
                return new BattleAction(ActionKind.Switch, SwitchPriority, player.Active.GetEffectiveSpeed(), SwitchIndex: switchIndex);
            }

            if (pick == 3)
            {
                if (player.Inventory.TotalCount == 0)
                {
                    _ctx.Ui.WriteLine("No usable items. Defaulting to Attack.");
                    var moveIndex = PickMove(player.Active);
                    var move = player.Active.Moves[moveIndex];
                    return new BattleAction(ActionKind.Attack, move.Priority, player.Active.GetEffectiveSpeed(), MoveIndex: moveIndex);
                }

                var item = PickItem(player);
                return new BattleAction(ActionKind.UseItem, ItemPriority, player.Active.GetEffectiveSpeed(), Item: item);
            }
        }
    }

    private void PrintParty(Trainer t)
    {
        _ctx.Ui.WriteLine();
        _ctx.Ui.WriteLine($"[{t.Name}] Party");
        for (var i = 0; i < t.Party.Count; i++)
        {
            var p = t.Party[i];
            var marker = i == t.ActiveIndex ? "*" : " ";
            var hp = p.IsFainted ? "FAINTED" : $"{p.CurrentHp}/{p.MaxHp}";
            var types = p.SecondaryType is null ? $"{p.PrimaryType}" : $"{p.PrimaryType}/{p.SecondaryType}";
            _ctx.Ui.WriteLine($"{marker} {i + 1}. {p.Name} [{types}] HP {hp} Status {p.Status}");
        }
        _ctx.Ui.WriteLine();
    }

    private int PickMove(SpeciesPokemon p)
    {
        _ctx.Ui.WriteLine();
        _ctx.Ui.WriteLine($"Pick a move: {p.Name}");
        for (var i = 0; i < p.Moves.Count; i++)
        {
            var m = p.Moves[i];
            var info =
                m is DamageMove dm ? $"Power {dm.Power} Acc {dm.Accuracy}% {dm.Category}" + (dm.StatusChance > 0 && dm.StatusOnHit != StatusCondition.None ? $" +{dm.StatusChance}% {dm.StatusOnHit}" : "") :
                m is RecoverMove rm ? $"Heal {rm.HealAmount}" + (rm.CuresStatus ? " + Cure" : "") :
                m is StatusMove sm ? $"Inflict {sm.StatusToApply} Acc {sm.Accuracy}%" :
                m is FieldMove fm ? $"SEt {fm.FieldEffect} Acc {fm.Accuracy}" :
                "Move";

            _ctx.Ui.WriteLine($"{i + 1}. {m.Name} [{m.Type}] (Prio {m.Priority}) {info}");
        }
        _ctx.Ui.WriteLine();
        return _ctx.Ui.ReadIntInRange("Number:", 1, p.Moves.Count) - 1;
    }

    private int PickSwitchIndex(Trainer t)
    {
        _ctx.Ui.WriteLine();
        _ctx.Ui.WriteLine("Pick a Pokemon to switch in.");
        for (var i = 0; i < t.Party.Count; i++)
        {
            var p = t.Party[i];
            var marker = i == t.ActiveIndex ? "*" : " ";
            var hp = p.IsFainted ? "FAINTED" : $"{p.CurrentHp}/{p.MaxHp}";
            var types = p.SecondaryType is null ? $"{p.PrimaryType}" : $"{p.PrimaryType}/{p.SecondaryType}";
            _ctx.Ui.WriteLine($"{marker} {i + 1}. {p.Name} [{types}] HP {hp} Status {p.Status}");
        }
        _ctx.Ui.WriteLine();

        while (true)
        {
            var pick = _ctx.Ui.ReadIntInRange("Number:", 1, t.Party.Count) - 1;
            if (t.CanSwitchTo(pick)) return pick;
            _ctx.Ui.WriteLine("Invalid switch selection.");
        }
    }

    private IItem PickItem(Trainer t)
    {
        var items = t.Inventory.ListItems();
        _ctx.Ui.WriteLine();
        _ctx.Ui.WriteLine("Pick an item.");
        for (var i = 0; i < items.Count; i++)
        {
            var (item, count) = items[i];
            _ctx.Ui.WriteLine($"{i + 1}. {item.Name} x{count}");
        }
        _ctx.Ui.WriteLine();

        while (true)
        {
            var idx = _ctx.Ui.ReadIntInRange("Number:", 1, items.Count) - 1;
            var (item, count) = items[idx];
            if (count <= 0)
            {
                _ctx.Ui.WriteLine("No stock left.");
                continue;
            }
            return item;
        }
    }

    private BattleAction DecideAiAction(Trainer ai, Trainer opponent, SideState aiSide, SideState opponentSide)
    {
        EnsureValidActive(ai);
        EnsureValidActive(opponent);

        var active = ai.Active;

        if (active.IsFainted)
        {
            var idx = ai.FirstAvailableIndex();
            return new BattleAction(ActionKind.Switch, SwitchPriority, active.GetEffectiveSpeed(), SwitchIndex: idx);
        }

        if (active.Status.Condition != StatusCondition.None &&
            ai.Inventory.TryFindItem<FullHeal>(out var fullHeal) &&
            ai.Inventory.GetCount(fullHeal) > 0)
        {
            var c = active.Status.Condition;
            if (c is StatusCondition.Sleep or StatusCondition.BadPoison or StatusCondition.Burn)
            {
                return new BattleAction(ActionKind.UseItem, ItemPriority, active.GetEffectiveSpeed(), Item: fullHeal);
            }
        }

        if (active.HpRatio() <= 0.30 &&
            ai.Inventory.TryFindItem<Potion>(out var potion) &&
            ai.Inventory.GetCount(potion) > 0)
        {
            return new BattleAction(ActionKind.UseItem, ItemPriority, active.GetEffectiveSpeed(), Item: potion);
        }

        // var bestMoveIndex = 0;
        // var bestScore = double.NegativeInfinity;

        // for (var i = 0; i < active.Moves.Count; i++)
        // {
        //     var m = active.Moves[i];
        //     var socre = ScoreMove(m, active, opponent.Active, opponentSide);
        //     if (socre > bestScore)
        //     {
        //         bestScore = socre;
        //         bestMoveIndex = i;
        //     }
        // }

        // var bestMove = active.Moves[bestMoveIndex];
        // return new BattleAction(ActionKind.Attack, bestMove.Priority, active.GetEffectiveSpeed(), MoveIndex: bestMoveIndex);

        var randomMoveIndex = _ctx.Rng.Next(active.Moves.Count);
        var randomMove = active.Moves[randomMoveIndex];

        return new BattleAction(
            ActionKind.Attack,
            randomMove.Priority,
            active.GetEffectiveSpeed(),
            MoveIndex: randomMoveIndex
        );
    }

    private double ScoreMove(IMove move, SpeciesPokemon user, SpeciesPokemon target, SideState opponentSide)
    {
        if (move is FieldMove fm)
        {
            if (fm.FieldEffect == FieldEffect.StealthRock)
            {
                return opponentSide.HasStealthRock ? 0.5 : 45.0;
            }
            return 0.5;
        }

        if (move is StatusMove sm)
        {
            if (target.Status.Condition != StatusCondition.None) return 0.1;
            if (!target.CanReceiveStatus(sm.StatusToApply)) return 0.1;

            return sm.StatusToApply switch
            {
                StatusCondition.Sleep => 35.0,
                StatusCondition.BadPoison => 30.0,
                StatusCondition.Paralysis => 25.0,
                StatusCondition.Burn => 22.0,
                StatusCondition.Poison => 18.0,
                _ => 10.0
            };
        }

        if (move is RecoverMove rm)
        {
            var missing = user.MaxHp - user.CurrentHp;
            return Math.Min(missing, rm.HealAmount) * 0.9;
        }

        if (move is DamageMove dm)
        {
            var eff = TypeChart.Effectiveness(dm.Type, target.PrimaryType, target.SecondaryType);
            if (eff <= 0.0) return 0.0;

            var stab = user.HasType(dm.Type) ? 1.5 : 1.0;
            var acc = dm.Accuracy / 100.0;

            var atk = user.GetEffectiveOffense(dm.Category);
            var def = target.GetEffectiveDefense(dm.Category);
            var baseDamage = dm.Power * (atk / (double)Math.Max(1, def));

            var expected = baseDamage * stab * eff * acc * 0.925;
            return expected;
        }

        return 0.0;
    }

    private List<(Trainer actor, Trainer target, BattleAction action, SideState actorSide, SideState targetSide)> OrderActions(
        Trainer player,
        Trainer enemy,
        BattleAction playerAction,
        BattleAction enemyAction)
    {
        var a = (actor: player, target: enemy, action: playerAction, actorSide: _playerSide, targetSide: _enemySide);
        var b = (actor: enemy, target: player, action: enemyAction, actorSide: _enemySide, targetSide: _playerSide);

        var list = new List<(Trainer actor, Trainer target, BattleAction action, SideState actorSide, SideState targetSide)> { a, b };

        list.Sort((x, y) =>
        {
            var pr = y.action.Priority.CompareTo(x.action.Priority);
            if (pr != 0) return pr;

            var sp = y.action.Priority.CompareTo(x.action.Speed);
            if (pr != 0) return sp;

            return _ctx.Rng.Next(-1, 2);
        });

        return list;
    }

    private void ExecuteAction(Trainer actor, Trainer target, BattleAction action, SideState actorSide, SideState targetSide)
    {
        EnsureValidActive(actor);
        EnsureValidActive(target);

        switch (action.Kind)
        {
            case ActionKind.Switch:
                ExecuteSwitch(actor, action.SwitchIndex, actorSide);
                return;

            case ActionKind.UseItem:
                ExecuteItem(actor, target, action.Item);
                return;

            case ActionKind.Attack:
                ExecuteAttack(actor, target, action.MoveIndex, actorSide, targetSide);
                return;

            default:
                return;
        }
    }

    private void ExecuteSwitch(Trainer actor, int? switchIndex, SideState actorSide)
    {
        if (switchIndex is null)
        {
            AutoSwitchIfNeeded(actor, actorSide);
            return;
        }

        if (!actor.CanSwitchTo(switchIndex.Value))
        {
            _ctx.Ui.WriteLine($"{actor.Name} failed to switch.");
            return;
        }

        actor.Active.OnSwitchOut();
        actor.TrySwitchTo(switchIndex.Value);
        _ctx.Ui.WriteLine($"{actor.Name} switched to {actor.Active.Name}.");

        ApplyEntryHazardsIfAny(actor, actorSide);
        AutoSwitchIfNeeded(actor, actorSide);
    }

    private void ExecuteItem(Trainer actor, Trainer target, IItem? item)
    {
        var chosen = item ?? new Potion();
        var ok = actor.Inventory.TryConsume(chosen);
        if (!ok)
        {
            _ctx.Ui.WriteLine($"{actor.Name} has no item to use");
            return;
        }

        chosen.Use(_ctx, actor, target);
    }

    private void ExecuteAttack(Trainer actor, Trainer target, int? moveIndex, SideState actorSide, SideState targetSide)
    {
        if (actor.Active.IsFainted) return;

        if (actor.Active.Status.Condition == StatusCondition.Sleep)
        {
            actor.Active.Status.DecrementSleep();
            if (actor.Active.Status.Condition == StatusCondition.Sleep)
            {
                _ctx.Ui.WriteLine($"{actor.Active.Name} is asleep and can't move!");
                return;
            }
            _ctx.Ui.WriteLine($"{actor.Active.Name} woke up!");
        }

        if (actor.Active.Status.Condition == StatusCondition.Paralysis)
        {
            var roll = _ctx.Rng.Next(1, 101);
            if (roll <= 25)
            {
                _ctx.Ui.WriteLine($"{actor.Active.Name} is fully paralyzed and can't mkove!");
                return;
            }
        }

        var idx = moveIndex ?? 0;
        idx = Math.Clamp(idx, 0, actor.Active.Moves.Count - 1);

        var move = actor.Active.Moves[idx];
        _ctx.Ui.WriteLine($"{actor.Name}'s {actor.Active.Name} used {move.Name}");

        var mc = new MoveContext(
            battle: _ctx,
            userTrainer: actor,
            targetTrainer: target,
            user: actor.Active,
            target: target.Active,
            userSide: actorSide,
            targetSide: targetSide
        );

        move.Execute(mc);

        if (target.Active.IsFainted)
        {
            _ctx.Ui.WriteLine($"{target.Name} is {target.Active.Name} fainted!");
            AutoSwitchIfNeeded(target, targetSide);
        }
    }

    private void ApplyEntryHazardsIfAny(Trainer t, SideState side)
    {
        if (!t.HasAvailablePokemon()) return;
        if (t.Active.IsFainted) return;

        if (side.HasStealthRock)
        {
            var eff = TypeChart.Effectiveness(PokemonType.Rock, t.Active.PrimaryType, t.Active.SecondaryType);
            if (eff <= 0.0)
            {
                _ctx.Ui.WriteLine($"{t.Active.Name} is unaffected by Stealth Rock!");
                return;
            }

            var baseDmg = t.Active.MaxHp / 8.0;
            var dmg = (int)Math.Floor(baseDmg * eff);
            dmg = Math.Max(1, dmg);

            t.Active.TakeDamage(dmg);
            _ctx.Ui.WriteLine($"{t.Active.Name} was hurt by Stealth Rock! (-{dmg} HP)");

            if (t.Active.IsFainted)
            {
                _ctx.Ui.WriteLine($"{t.Name}'s {t.Active.Name} fainted from Stealth Rock!");
            }
        }
    }

    private void ApplyEndOfTurnEffects(Trainer t, SideState side)
    {
        if (!t.HasAvailablePokemon()) return;
        if (t.Active.IsFainted) return;

        var p = t.Active;
        var c = p.Status.Condition;

        if (c == StatusCondition.Burn)
        {
            var dmg = Math.Max(1, p.MaxHp / 8);
            p.TakeDamage(dmg);
            _ctx.Ui.WriteLine($"{p.Name} is hurt by its burn! (-{dmg} HP)");
        }
        else if (c == StatusCondition.Poison)
        {
            var dmg = Math.Max(1, p.MaxHp / 8);
            p.TakeDamage(dmg);
            _ctx.Ui.WriteLine($"{p.Name} is hurt by its poison! (-{dmg} HP)");
        }
        else if (c == StatusCondition.BadPoison)
        {
            var stage = Math.Max(1, p.Status.ToxicStage);
            var dmg = (int)Math.Floor(p.MaxHp * (stage / 16.0));
            dmg = Math.Max(1, dmg);
            p.TakeDamage(dmg);
            _ctx.Ui.WriteLine($"{p.Name} is hurt by its toxic poison! (-{dmg} HP)");
            p.Status.AdvanceToxicStage();
        }

        if (p.IsFainted)
        {
            _ctx.Ui.WriteLine($"{t.Name}'s {p.Name} fainted!");
            AutoSwitchIfNeeded(t, side);
        }
    }

    private void AutoSwitchIfNeeded(Trainer t, SideState side)
    {
        while (t.HasAvailablePokemon() && t.Active.IsFainted)
        {
            var idx = t.FirstAvailableIndex();
            if (idx < 0) return;

            t.TrySwitchTo(idx);
            _ctx.Ui.WriteLine($"{t.Name} switched to {t.Active.Name}.");
            ApplyEntryHazardsIfAny(t, side);
        }
    }
}