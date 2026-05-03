namespace PokemonCliBattleManager.Battle;

public sealed class SideState
{
    public bool HasStealthRock { get; private set; }
    public bool TrySetStealthRock()
    {
        if (HasStealthRock) return false;
        HasStealthRock = true;
        return true;
    }

    public void ClearAll()
    {
        HasStealthRock = false;
    }
}