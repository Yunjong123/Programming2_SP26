using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
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

        private static string Key(string s)
        {
            return (s ?? "").Trim().ToLowerInvariant();
        }

        public int SearchCollectionByName(string itemName)
        {
            string k = Key(itemName);
            if (k.Length == 0) return -1;

            for (int i = 0; i < items.Count; i++)
            {
                if (Key(items[i].Name) == k) return i;
            }
            return -1;
        }

        public bool Contains(string itemName)
        {
            return SearchCollectionByName(itemName) >= 0;
        }

        public double AmountOf(string itemName)
        {
            int idx = SearchCollectionByName(itemName);
            if (idx < 0) return 0.0;
            return items[idx].Amount;
        }

        public Item FindByName(string itemName)
        {
            int idx = SearchCollectionByName(itemName);
            if (idx < 0) return null;
            return items[idx];
        }

        public void AddToCollectionByName(Item item)
        {
            if (item == null) return;
            if (string.IsNullOrWhiteSpace(item.Name)) return;
            if (item.Amount <= 0) return;

            int idx = SearchCollectionByName(item.Name);
            if (idx < 0)
            {
                items.Add(item.Clone());
                return;
            }

            items[idx].Amount += item.Amount;

            if (string.IsNullOrWhiteSpace(items[idx].AmountType)) items[idx].AmountType = item.AmountType;
            if (items[idx].Value <= 0) items[idx].Value = item.Value;
            if (string.IsNullOrWhiteSpace(items[idx].Description)) items[idx].Description = item.Description;
        }

        public bool RemoveFromCollectionByIndexNumber(int itemIndexNumber)
        {
            if (itemIndexNumber < 0 || itemIndexNumber >= items.Count) return false;
            items.RemoveAt(itemIndexNumber);
            return true;
        }

        public bool RemoveAmountByName(string itemName, double amount)
        {
            if (amount <= 0) return false;

            int idx = SearchCollectionByName(itemName);
            if (idx < 0) return false;

            if (items[idx].Amount < amount) return false;

            items[idx].Amount -= amount;
            if (items[idx].Amount <= 0)
            {
                items.RemoveAt(idx);
            }
            return true;
        }

        public bool ChangeAmountByName(string itemName, double delta)
        {
            if (string.IsNullOrWhiteSpace(itemName)) return false;
            if (delta == 0) return true;

            int idx = SearchCollectionByName(itemName);

            if (idx < 0)
            {
                if (delta <= 0) return false;

                items.Add(new Item
                {
                    Name = itemName.Trim(),
                    Amount = delta,
                    AmountType = "unit(s)",
                    Value = 0.0,
                    Description = ""
                });
                return true;
            }

            items[idx].Amount += delta;
            if (items[idx].Amount <= 0)
            {
                items.RemoveAt(idx);
            }
            return true;
        }

        public bool HasEnough(string itemName, double amount)
        {
            int idx = SearchCollectionByName(itemName);
            if (idx < 0) return false;
            return items[idx].Amount >= amount;
        }
    }
}