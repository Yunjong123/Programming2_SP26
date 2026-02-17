using System.Collections.Generic;
using System.Security.Cryptography;

namespace CraftingSystemMonday
{
    public static class TestData
    {
        public static List<Item> PlayerStarterItems()
        {
            return new List<Item>
            {
                new Item { Name = "Water", Amount = 5,
                AmountType = "cup(s)", Value = 0.10,
                Description = "water" },
                new Item { Name = "Chamomile", Amount = 3,
                AmountType = "tsp(s)", Value = 0.25,
                Description = "Matricaria recutita, dried" },
                new Item { Name = "Honey", Amount = 2,
                AmountType = "tbsp(s)", Value = 0.40,
                Description = "honey" },
            };
        }
        public static List<Item> TraderStarterItems()
        {
            return new List<Item>
            {
                new Item { Name = "Water", Amount = 20,
                AmountType = "cup(s)", Value = 0.10,
                Description = "water" },
                new Item { Name = "Honey", Amount = 10,
                AmountType = "tbsp(s)", Value = 0.40,
                Description = "honey" },
                new Item { Name = "Lemmon", Amount = 10,
                AmountType = "slice(s)", Value = 0.15,
                Description = "lemon" },
            };
        }
        public static Recipe EngineStartRecipe()
        {
            List<Item> req = new List<Item>();
            foreach (Item it in PlayerStarterItems())
            {
                req.Add(new Item
                {
                    Name = it.Name,
                    Amount = it.Amount,
                    AmountType = it.AmountType,
                    Value = it.Value,
                    Description = it.Description
                });
            }

            return new Recipe(
                "Starter Brew",
                "Temporary test recipe. Requirements mnatch the player's starting items (same amounts)",
                req,
                outputAmount: 1,
                value: 5.00
            );
        }
    }
}