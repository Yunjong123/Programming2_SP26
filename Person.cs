using System.Collections.Generic;
using System.Linq;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Person
    {
        public string Name { get; protected set; } = "Default";
        public double Currency { get; set; } = 25.00;

        public Inventory Inventory { get; } = new Inventory();

        public Person() { }

        public Person(string name, double currency)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Default" : name.Trim();
            Currency = currency;
        }

        public virtual void UpdateName()
        {
            string input = GetInput("Enter a new name:");
            if (!string.IsNullOrWhiteSpace(input))
            {
                Name = input.Trim();
            }
        }

        public string Information()
        {
            return $"Name: {Name}\nCurrency: {Currency.ToString("C")}";
        }

        public void PrintInventory()
        {
            if (Inventory.Count == 0)
            {
                Print("Inventory is epmty.");
                return;
            }

            Print("Inventory:");
            var list = Inventory.Items();
            for (int i = 0; i < list.Count; i++)
            {
                Item it = list[i];
                Print($"  {i + 1}: {it.Name} - {it.Amount} {it.AmountType} | each {it.Value.ToString("C")}");
            }
        }
    }
}