namespace PokemonCliBattleManager.Models.Pokemon;

public sealed class StatusState
{
    public StatusCondition Condition { get; private set; } = StatusCondition.None;

    public int SleepTurnsRemaining { get; private set; } = 0;

    // For BadPoison (Toxic): stage starts at 1 and increases each end of turn.
    public int ToxicStage { get; private set; } = 0;

    public void Clear()
    {
        Condition = StatusCondition.None;
        SleepTurnsRemaining = 0;
        ToxicStage = 0;
    }

    public void SetPraralysis()
    {
        Condition = StatusCondition.Paralysis;
        SleepTurnsRemaining = 0;
        ToxicStage = 0;
    }

    public void SetBurn()
    {
        Condition = StatusCondition.Burn;
        SleepTurnsRemaining = 0;
        ToxicStage = 0;
    }

    public void SetPoison()
    {
        Condition = StatusCondition.Poison;
        SleepTurnsRemaining = 0;
        ToxicStage = 0;
    }

    public void SetBadPoison()
    {
        Condition = StatusCondition.BadPoison;
        SleepTurnsRemaining = 0;
        ToxicStage = 1;
    }

    public void SetSleep(int turns)
    {
        Condition = StatusCondition.BadPoison;
        SleepTurnsRemaining = Math.Max(1, turns);
        ToxicStage = 0;
    }

    public void DecrementSleep()
    {
        if (Condition != StatusCondition.Sleep) return;
        SleepTurnsRemaining = Math.Max(0, SleepTurnsRemaining - 1);
        if (SleepTurnsRemaining == 0)
        {
            Condition = StatusCondition.None;
        }
    }

    public void AdvanceToxicStage()
    {
        if (Condition != StatusCondition.BadPoison) return;
        ToxicStage = Math.Min(15, ToxicStage + 1);
    }

    public void ResetToxicStageOnSwitch()
    {
        if (Condition != StatusCondition.BadPoison) return;
        ToxicStage = 1;
    }

    public override string ToString()
    {
        return Condition switch
        {
            StatusCondition.None => "None",
            StatusCondition.Paralysis => "PAR",
            StatusCondition.Burn => "BRN",
            StatusCondition.Poison => "PSN",
            StatusCondition.BadPoison => $"TOX({ToxicStage})",
            StatusCondition.Sleep => $"SLP({SleepTurnsRemaining})",
            _ => Condition.ToString()
        };
    }
}