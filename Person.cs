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

        public Person() : this("Default Person", 25.00) { }

        public Person(string name, double currency)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Default Person" : name.Trim();
            Currency = currency;

            SeedTemporaryInventoryForTesting();
        }

        public virtual void UpdateName()
        {
            string input = GetInput("Enter a new player name:");
            if (!String.IsNullOrWhiteSpace(input))
            {
                Name = input.Trim();
            }
        }

        public string HubLine()
        {
            return $"Player: {Name} | Currency: {Currency.ToString("C")}";
        }

        private void SeedTemporaryInventoryForTesting()
        {
            Inventory.AddToCollectionByName(new Item { Name = "Water", Amount = 5, AmountType = "cup(s)", Value = 0.10, Description = "water" });
            Inventory.AddToCollectionByName(new Item { Name = "Chamomile", Amount = 3, AmountType = "tsp(s)", Value = 0.25, Description = "dried chamomile" });
            Inventory.AddToCollectionByName(new Item { Name = "Honey", Amount = 2, AmountType = "tbsp(s)", Value = 0.25, Description = "honey" });
        }
    }
}