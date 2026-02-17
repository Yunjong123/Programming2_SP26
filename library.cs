using System;

namespace CraftingSystemMonday
{
    public static class Library
    {
        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static void SetTitle(string title)
        {
            Console.Title = title ?? "";
        }

        public static string GetInput(string prompt = null)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Print(prompt);
            }

            string input = Console.ReadLine();
            return (input ?? "").Trim();
        }

        public static bool IsNumber(string input)
        {
            return double.TryParse((input ?? "").Trim(), out _);
        }

        public static bool IsWholeNumber(string input)
        {
            return int.TryParse((input ?? "").Trim(), out _);
        }

        public static int ConvertStringToInteger(string input, int fallback = -1)
        {
            return int.TryParse((input ?? "").Trim(), out int value) ? value : fallback;
        }

        public static double ConvertStringToDouble(string input, double fallback = -1)
        {
            return double.TryParse((input ?? "").Trim(), out double value) ? value : fallback;
        }

        public static int SearchCollectionByName(List<Item> list, string itemName)
        {
            return SearchCollectionByName(list, itemName, partialMatch: false);
        }

        public static int SearchCollectionByName(List<Item> list, string itemName, bool partialMatch)
        {
            if (list == null || list.Count == 0) return -1;
            if (string.IsNullOrWhiteSpace(itemName)) return -1;

            string key = itemName.Trim();

            for (int i = 0; i < list.Count; i++)
            {
                string current = (list[i]?.Name ?? "").Trim();
                if (current.Length == 0) continue;

                if (!partialMatch)
                {
                    if (string.Equals(current, key, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
                else
                {
                    if (current.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static bool SearchCollectionByName(List<Item> list, string itemName, out int index)
        {
            index = SearchCollectionByName(list, itemName);
            return index >= 0;
        }

        public static void AddToCollectionByName(List<Item> list, Item item)
        {
            if (list == null) return;
            if (item == null) return;
            if (string.IsNullOrWhiteSpace(item.Name))
                return;
            if (item.Amount <= 0) return;

            int index = SearchCollectionByName(list, item.Name);
            if (index < 0)
            {
                list.Add(item.Clone());
                return;
            }

            Item existing = list[index];
            existing.Amount += item.Amount;

            if (string.IsNullOrWhiteSpace(existing.AmountType)) existing.AmountType = item.AmountType;
            if (existing.Value <= 0) existing.Value = item.Value;
            if (string.IsNullOrWhiteSpace(existing.Description)) existing.Description = item.Description;
        }

        public static bool RemoveFromCollectionByIndexNumber(List<Item> list, int itemIndexNumber)
        {
            if (list == null || list.Count == 0) return false;

            int idx = itemIndexNumber;

            if (idx >= 1 && idx <= list.Count)
            {
                idx = idx - 1;
            }

            if (idx < 0 || idx >= list.Count) return false;

            list.RemoveAt(idx);
            return true;
        }
    }
}