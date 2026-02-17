using System.Collections.Generic;
using static CraftingSystemMonday.Library;

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

            int index = SearchCollectionByName(name);
            if (index < 0) return null;

            return items[index];
        }

        public int SearchCollectionByName(string itemName)
        {
            return Library.SearchCollectionByName(items, itemName);
        }

        public bool SearchCollectionsByName(string itemName, out int index)
        {
            return Library.SearchCollectionByName(items, itemName, out index);
        }

        public void Add(Item item)
        {
            AddToCollectionByName(item);
        }

        public void AddToCollectionByName(Item item)
        {
            Library.AddToCollectionByName(items, item);
        }

        public bool RemoveFromCollectionByIndexNumber(int itemIndexNumber)
        {
            return Library.RemoveFromCollectionByIndexNumber(items, itemIndexNumber);
        }

        public bool RemoveByNameAmount(string name, double amount)
        {
            if (amount <= 0) return false;

            int index = SearchCollectionByName(name);
            if (index < 0) return false;

            Item existing = items[index];
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
            int index = SearchCollectionByName(name);
            if (index < 0) return false;

            return items[index].Amount >= amount;
        }

        public void PrintAll()
        {
            if (Count == 0)
            {
                Print("Inventory is empty");
                return;
            }

            Print("Inventory:");
            for (int i = 0; i < items.Count; i++)
            {
                Item it = items[i];
                Print($" {i + 1}. {it.Name} - {it.Amount} {it.AmountType} | each {it.Value.ToString("C")}");
            }
        }

    }
}