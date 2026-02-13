using System.Collections.Generic;
using System.Linq;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Person
    {
        public string Name { get; protected set; } = "Default Player";
        public double Currency { get; set; } = 25.00;
        public Inventory Inventory { get; } = new Inventory();
        public Person() { }

        public Person(string name, double currency = 25.00)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Default Player" : name.Trim();
            Currency = currency;
        }

        public virtual void UpdateName()
        {
            string input = GetInput("Enter a new player name:");
            if (!string.IsNullOrWhiteSpace(input))
            {
                Name = input.Trim();
            }
        }

        public string Information()
        {
            return $"Player: {Name}\nCurrency: {Currency.ToString("c")}";
        }

        public void PrintInventory()
        {
            if (Inventory.Count == 0)
            {
                Print("Inventory is empty");
                return;
            }

            Print("Inventory:");
            int i = 1;
            foreach (Item item in Inventory.Items())
            {
                Print($" {i}. {item.Name} - {item.Amount} {item.AmountType} | each {item.Value.ToString("C")}");
                i++;
            }

        }
    }
}