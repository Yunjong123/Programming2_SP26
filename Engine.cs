using System.Collections.Generic;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Engine
    {
        private readonly Player player = new Player("Default player", 25.00);

        private readonly Trader vendor = new Trader("Vendor", 0.00);

        public List<Recipe> Recipes { get; } = new List<Recipe>();

        private readonly string title = "Awesome Potion Crafitng System";
        private readonly string credits = "Credits: (edit Program.cs comments)";
        public Engine()
        {
            SeedTemporaryRecipeForTesting();
        }
        public void Start()
        {
            Setup();
            GameLoop();
        }

        public void Setup()
        {
            SetTitle(title);
            Print(title);
            Print(credits);
            Print("");
        }

        public void GameLoop()
        {
            while (true)
            {
                PrintHub();
                Print("");
                Print(MenuText());

                int choice = ConvertStringToInteger(GetInput("Enter a number:"), fallback: -1);

                switch (choice)
                {
                    case 0:
                        Print("Goodbye!");
                        return;
                    case 1:
                        player.Inventory.PrintAll();
                        break;
                    case 2:
                        ShowAllRecipes();
                        break;
                    case 3:
                        RecipeMenu();
                        break;
                    case 4:
                        Craftmenu();
                        break;
                    case 5:
                        player.UpdateName();
                        break;
                    case 6:
                        SearchInventoryMenu();
                        break;
                    case 7:
                        AddItemMenu();
                        break;
                    case 8:
                        RemoveItemMenu();
                        break;

                    case 9:
                        Print(credits);
                        break;

                    default:
                        Print("Please enter a valid menu option.");
                        break;

                }
            }
        }

        public void PrintHub()
        {
            Print(player.HubLine());
        }
        private string MenuText()
        {
            string output = "choose an option:";
            output += "\n1. show Inventory";
            output += "\n2. Show Recipes";
            output += "\n3. See recipe details";
            output += "\n4. Craft";
            output += "\n5. Change playter name";
            output += "\n6. Search inventory (Optional Challenge)";
            output += "\n7. Add item to inventory (Optional Challenge)";
            output += "\n8. Remove item from inventory (Optional Challenge)";
            output += "\n9. Credits";
            output += "\n0. Exit";
            output += $"\n\n{credits}";
            return output;
        }

        private void SeedTemporaryRecipeForTesting()
        {
            Recipes.Clear();

            Recipes.Add(
                new Recipe(
                    "Starter Brew",
                    "Temporary testing recipe. Requirements matchthe starter inventory amounts",
                    new List<Item>()
                    {
                        new Item(){ Name = "Water", Amount = 5, Value = 0.10, Description = "Water", AmountType = "cup(s)" },
                        new Item(){ Name = "Chamomile", Amount = 3, Value = 0.25, Description = "dried chamomile", AmountType = "tsp(s)" },
                        new Item(){ Name = "Honey", Amount = 2, Value = 0.40, Description = "honey", AmountType = "tbsp(s)" },
                    },
                    outputAmount: 1,
                    value: 5.00
                )
            );
        }

        public void ShowAllRecipes()
        {
            if (Recipes.Count == 0)
            {
                Print("No recipes available.");
                return;
            }

            Print("Available Recipes:");
            for (int i = 0; i < Recipes.Count; i++)
            {
                Print($" {i + 1}. {Recipes[i].Information()}");
            }
        }

        public void RecipeMenu()
        {
            ShowAllRecipes();
            int num = ConvertStringToInteger(GetInput("Enter the number of the recipe you would like to view:"));
            if (num >= 1 && num <= Recipes.Count)
            {
                Print(Recipes[num - 1].Details());
                return;
            }

            Print($"Please enter a number between 1 and {Recipes.Count}.");
        }

        private void Craftmenu()
        {
            ShowAllRecipes();

            int num = ConvertStringToInteger(GetInput("Enter the number of the recipe you would like craft:"));
            if (num < 1 || num > Recipes.Count)
            {
                Print($"Please enter a number between 1 and {Recipes.Count}.");
                return;
            }

            Recipe recipe = Recipes[num - 1];

            if (!CanCraft(recipe, out string reason))
            {
                Print($"Cannot craft {recipe.Name}: {reason}");
                return;
            }

            foreach (Item req in recipe.RequiredItems)
            {
                player.Inventory.RemoveByNameAmount(req.Name, req.Amount);
            }

            player.Inventory.Add(recipe.CreateOutputItem());
            Print($"Crafted: {recipe.Name} x{recipe.OutputAmount}");
        }

        private bool CanCraft(Recipe recipe, out string reason)
        {
            foreach (Item req in recipe.RequiredItems)
            {
                if (!player.Inventory.hasEnough(req.Name, req.Amount))
                {
                    reason = $"missing {req.Name} ({req.Amount} {req.AmountType})";
                    return false;
                }
            }

            reason = "";
            return true;
        }

        private void SearchInventoryMenu()
        {
            string name = GetInput("Enter an item name to search for:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            int index = player.Inventory.SearchCollectionByName(name);

            if (index < 0)
            {
                Print("No matching item found.");
                return;
            }

            Item found = player.Inventory.Items()[index];
            Print("Found:");
            Print($" {found.Information()}");
        }

        private void AddItemMenu()
        {
            string name = GetInput("Item name:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            string amountText = GetInput("Amount (number):");
            double amount = ConvertStringToDouble(amountText, fallback: -1);
            if (amount <= 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            string amountType = GetInput("Amount type (ex: cup(s), unit(s)) [press Enter for unit(s)]:");
            if (string.IsNullOrWhiteSpace(amountType))
            {
                amountType = "unit(s)";
            }

            string valueText = GetInput("Value per item (number) [press Enter for 0]:");
            double value = 0.0;
            if (!string.IsNullOrWhiteSpace(valueText))
            {
                value = ConvertStringToDouble(valueText, fallback: -1);
                if (value < 0)
                {
                    Print("Value must be 0 or a positive number.");
                    return;
                }
            }

            string description = GetInput("Description [optional]:");

            player.Inventory.AddToCollectionByName(new Item
            {
                Name = name.Trim(),
                Amount = amount,
                AmountType = amountType.Trim(),
                Value = value,
                Description = (description ?? "").Trim()
            });

            Print("Addded/updated inventory item.");
            player.Inventory.PrintAll();
        }

        private void RemoveItemMenu()
        {
            string name = GetInput("Enter an item name to remove:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            int index = player.Inventory.SearchCollectionByName(name);
            if (index < 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            bool ok = player.Inventory.RemoveFromCollectionByIndexNumber(index);
            if (!ok)
            {
                Print("Remove failed.");
                return;
            }

            Print("Removed item amount.");
            player.Inventory.PrintAll();
        }
    }
}