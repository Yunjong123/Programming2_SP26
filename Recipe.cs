using System.Collections.Generic;

namespace CraftingSystemMonday
{
    public class Recipe
    {
        public string Name { get; set; } = "Unnamed Recipe";

        public string Description { get; set; } = "";

        public List<Item> RequiredItems { get; set; } = new List<Item>();

        public double OutputAmount { get; set; } = 1.0;
        public double Value { get; set; } = 0.0;

        public Recipe() { }

        public Recipe(String name, string description, List<Item> requiredItems, double outputAmount = 1.0, double value = 0.0)
        {
            Name = name;
            Description = description;
            RequiredItems = requiredItems ?? new List<Item>();
            OutputAmount = outputAmount;
            Value = Value;
        }

        public string Information()
        {
            return $"{Name} (Makes {OutputAmount}, Value {Value.ToString("C")})";
        }

        public string Details()
        {
            string text = $"{Name}\n{Description}\nMakes: {OutputAmount}\nValue: {Value.ToString("C")}\nRequired:";
            if (RequiredItems == null || RequiredItems.Count == 0)
            {
                return text + "\n (none)";
            }

            foreach (Item item in RequiredItems)
            {
                text += $"\n - {item.Name}: {item.Amount} {item.AmountType}";
            }

            return text;
        }

        public Item CreateOutputItem()
        {
            return new Item
            {
                Name = Name,
                Description = Description,
                Amount = OutputAmount,
                AmountType = "unit(s)",
                Value = Value
            };
        }
    }
}