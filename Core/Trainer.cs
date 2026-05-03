using PokemonCliBattleManager.Items;
using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Core;

public sealed class Trainer
{
    public string Name { get; }
    public bool IsHuman { get; }
    public List<SpeciesPokemon> Party { get; } = new();
    public Inventory Inventory { get; } = new();
    public int ActiveIndex { get; private set; } = 0;

    public Trainer(string name, bool isHuman)
    {
        Name = name;
        IsHuman = isHuman;
    }

    public SpeciesPokemon Active => Party[ActiveIndex];
    public bool HasAvailablePokemon() => Party.Any(p => !p.IsFainted);
    public int FirstAvailableIndex()
    {
        for (var i = 0; i < Party.Count; i++)
        {
            if (!Party[i].IsFainted) return i;
        }
        return -1;
    }

    public IReadOnlyList<int> SwitchableIndices()
    {
        var list = new List<int>();
        for (var i = 0; i < Party.Count; i++)
        {
            if (i == ActiveIndex) continue;
            if (!Party[i].IsFainted) list.Add(i);
        }
        return list;
    }

    public bool CanSwitchTo(int index)
    {
        if (index < 0 || index >= Party.Count) return false;
        if (index == ActiveIndex) return false;
        if (Party[index].IsFainted) return false;
        return true;
    }

    public bool TrySwitchTo(int index)
    {
        if (!CanSwitchTo(index)) return false;
        ActiveIndex = index;
        return true;
    }
}