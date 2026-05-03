using System.Runtime.CompilerServices;

namespace PokemonCliBattleManager.Items;

public sealed class Inventory
{
    private readonly Dictionary<string, (IItem item, int count)> _items = new(StringComparer.OrdinalIgnoreCase);

    public int TotalCount => _items.Values.Sum(x => x.count);

    public void Add(IItem item, int count)
    {
        if (count <= 0) return;

        if (_items.TryGetValue(item.Name, out var cur))
        {
            _items[item.Name] = (cur.item, cur.count + count);
            return;
        }

        _items[item.Name] = (item, count);
    }

    public int GetCount(IItem item)
    {
        if (_items.TryGetValue(item.Name, out var cur)) return cur.count;
        return 0;
    }

    public bool TryConsume(IItem item)
    {
        if (!_items.TryGetValue(item.Name, out var cur)) return false;
        if (cur.count <= 0) return false;

        var next = cur.count - 1;
        if (next <= 0) _items.Remove(item.Name);
        else _items[item.Name] = (cur.item, next);

        return true;
    }

    public List<(IItem item, int count)> ListItems()
    {
        return _items.Values
            .Select(v => (v.item, v.count))
            .OrderByDescending(x => x.count)
            .ThenBy(x => x.item.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public bool TryFindItem<T>(out T found) where T : class, IItem
    {
        foreach (var v in _items.Values)
        {
            if (v.item is T t)
            {
                found = t;
                return true;
            }
        }

        found = null!;
        return false;
    }
}