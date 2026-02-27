using System.Collections.Generic;
using System.Dynamic;

namespace CraftingSystemMonday
{
    public class Recipe
    {
        public string Name { get; set; } = "Unnamed Recipe";
        public string Description { get; set; } = "";

        public List<Item> RequiredItems { get; set; } = new List<Item>();

        public double OutputAmount { get; set; } = 1.0;

        public string OutputAmountType { get; set; } = "unit(s)";
        public double OutputValueEach { get; set; } = 0.0;

        public string Information()
        {
            return $"{Name} (Makes {OutputAmount} {OutputAmountType})";
        }

        public string Details()
        {
            string text = $"{Name}\n{Description}\nMakes: {OutputAmount} {OutputAmountType}\nValue (each): {OutputValueEach.ToString("C")}\nRequired:";
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
                AmountType = OutputAmountType,
                Value = OutputValueEach
            };
        }
    }
}