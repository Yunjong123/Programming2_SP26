using PokemonCliBattleManager.Models.Moves;

namespace PokemonCliBattleManager.Models.Pokemon;

public abstract class Pokemon
{
    public string Name { get; }
    public PokemonType PrimaryType { get; }
    public PokemonType? SecondaryType { get; }

    public int MaxHp { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int SpAttack { get; }
    public int SpDefense { get; }
    public int Speed { get; }

    public int CurrentHp { get; private set; }

    public StatusState Status { get; } = new();

    public List<IMove> Moves { get; }

    public bool IsFainted => CurrentHp <= 0;

    protected Pokemon(
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
    {
        Name = name;
        PrimaryType = primaryType;
        SecondaryType = secondaryType;

        MaxHp = Math.Max(1, maxHp);
        Attack = Math.Max(1, attack);
        Defense = Math.Max(1, defense);
        spAttack = Math.Max(1, spAttack);
        spDefense = Math.Max(1, spDefense);
        Speed = Math.Max(1, speed);

        Moves = moves.ToList();
        if (Moves.Count == 0) throw new ArgumentException("At least 1 move is required.");

        CurrentHp = MaxHp;
    }

    public bool HasType(PokemonType t) => PrimaryType == t || SecondaryType == t;

    public void TakeDamage(int amount)
    {
        if (amount < 0) amount = 0;
        CurrentHp = Math.Max(0, CurrentHp - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
    }

    public void OnSwitchOut()
    {
        Status.ResetToxicStageOnSwitch();
    }

    public void ClearStatus()
    {
        Status.Clear();
    }

    public bool CanReceiveStatus(StatusCondition condition)
    {
        if (IsFainted) return false;
        if (Status.Condition != StatusCondition.None) return false;

        return condition switch
        {
            StatusCondition.Burn => !HasType(PokemonType.Fire),
            StatusCondition.Paralysis => !HasType(PokemonType.Electric),
            StatusCondition.Poison => !HasType(PokemonType.Steel) || HasType(PokemonType.Poison),
            StatusCondition.BadPoison => !HasType(PokemonType.Steel) || HasType(PokemonType.Poison),
            StatusCondition.Sleep => true,
            _ => false
        };
    }

    public bool TryApplyStatus(StatusCondition condition, Random rng, out string message)
    {
        message = "";

        if (condition == StatusCondition.None) return false;

        if (Status.Condition != StatusCondition.None)
        {
            message = $"{Name} is already affected by a status condition.";
            return false;
        }

        if (!CanReceiveStatus(condition))
        {
            message = $"{Name} is immune to {condition}.";
            return false;
        }

        switch (condition)
        {
            case StatusCondition.Paralysis:
                Status.SetPraralysis();
                message = $"{Name} is paralyzed! It may be unable to move!";
                return true;

            case StatusCondition.Burn:
                Status.SetBurn();
                message = $"{Name} was burned!";
                return true;

            case StatusCondition.Poison:
                Status.SetPoison();
                message = $"{Name} was poisoned!";
                return true;

            case StatusCondition.BadPoison:
                Status.SetBadPoison();
                message = $"{Name} was badly poisoned!";
                return true;

            case StatusCondition.Sleep:
                var turns = rng.Next(1, 4); //simplified for playability
                Status.SetSleep(turns);
                message = $"{Name} fell asleep!";
                return true;

            default:
                return false;
        }
    }

    public int GetEffectiveSpeed()
    {
        if (Status.Condition == StatusCondition.Paralysis)
        {
            // Gen 4 Paralysis: speed Quartered
            var s = (int)Math.Floor(Speed * 0.25);
            return Math.Max(1, s);
        }
        return Speed;
    }

    public int GetEffectiveAttack(MoveCategory category)
    {
        if (Status.Condition == StatusCondition.Burn && category == MoveCategory.Physical)
        {
            // Gen 4 burn: physical attack halved
            var a = (int)Math.Floor(Attack * 0.5);
            return Math.Max(1, a);
        }
        return Attack;
    }

    public int GetEffectiveDefense(MoveCategory category)
    {
        return category == MoveCategory.Physical ? Defense : SpDefense;
    }

    public int GetEffectiveOffense(MoveCategory category)
    {
        return category == MoveCategory.Physical ? GetEffectiveAttack(category) : SpAttack;
    }

    public double HpRatio() => CurrentHp / (double)MaxHp;
}