using System.Collections.Generic;

namespace CraftingSystemMonday
{
    public class Inventory
    {
        private readonly List<Item> items = new List<Item>();

        public int Count => items.Count;

        public List<Item> Items()
        {
            return new List<Item>(items);
        }

        public Item FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            string key = name.Trim().ToLowerInvariant();
            foreach (Item it in items)
            {
                string current = (it.Name ?? "").Trim().ToLowerInvariant();
                if (current == key) return it;
            }

            return null;
        }

        public void Add(Item item)
        {
            if (item == null) return;
            if (string.IsNullOrWhiteSpace(item.Name)) return;
            if (item.Amount <= 0) return;

            Item existing = FindByName(item.Name);
            if (existing == null)
            {
                items.Add(item.Clone());
                return;
            }

            existing.Amount += item.Amount;

            if (string.IsNullOrWhiteSpace(existing.AmountType)) existing.AmountType = item.AmountType;
            if (existing.Value <= 0) existing.Value = item.Value;
            if (string.IsNullOrWhiteSpace(existing.Description)) existing.Description = item.Description;
        }

        public bool Remove(string name, double amount)
        {
            if (amount <= 0) return false;

            Item existing = FindByName(name);
            if (existing == null) return false;
            if (existing.Amount < amount) return false;

            existing.Amount -= amount;

            if (existing.Amount <= 0)
            {
                items.Remove(existing);
            }

            return true;
        }

        public bool hasEnough(string name, double amount)
        {
            Item existing = FindByName(name);
            if (existing == null) return false;
            return existing.Amount >= amount;
        }

    }
}