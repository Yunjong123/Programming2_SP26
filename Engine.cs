using System.Collections.Generic;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Engine
    {
        private readonly Player player = new Player("Default player", 25.00);

        private readonly Trader vendor = new Trader("Vendor", 0.00);

        public List<Recipe> Recipes { get; } = new List<Recipe>();

        private readonly string appTitle = "Awesome Potion Crafitng System";
        private readonly string creditsLine = "Credits: (edit Program.cs comments)";
        public Engine()
        {
            Recipes.Add(TestData.EngineStartRecipe());
        }
        public void Start()
        {
            Setup();
            GameLoop();
        }

        public void Setup()
        {
            Console.Title = appTitle;
            Print(appTitle);
        }

        public void PrintHub()
        {
            Print("");
            Print($"Player: {player.Name}");
            Print($"Currency: {player.Currency.ToString("C")}");
        }

        public void GameLoop()
        {
            while (true)
            {
                Print("");
                Print(MenuText());

                int choice = ConvertStringToInteger(GetInput("Enter a number:"));

                switch (choice)
                {
                    case 0:
                        Print("Goodbye!");
                        return;
                    case 1:
                        player.PrintInventory();
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
                        PrintCredits();
                        break;

                    default:
                        Print("Please enter a valid menu option.");
                        break;

                }
            }
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
            return output;
        }

        private void PrintCredits()
        {
            Print("");
            Print("Credits");
            Print("This project is based on the Crafting System assignment.");
            Print("Make sure Program.cs includes your name/ course credits in comments.");
        }

        public void ShowAllRecipes()
        {
            if (Recipes.Count == 0)
            {
                Print("No recipes available.");
                return;
            }

            Print("Available Recipes:");
            int number = 1;
            foreach (Recipe recipe in Recipes)
            {
                Print($" {number}. {recipe.Information()}");
                number++;
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
                player.Inventory.Remove(req.Name, req.Amount);
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

            Item found = player.Inventory.FindByName(name);
            if (found == null)
            {
                Print("No matching item found.");
                return;
            }

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

            player.Inventory.Add(new Item
            {
                Name = name.Trim(),
                Amount = amount,
                AmountType = amountType.Trim(),
                Value = value,
                Description = (description ?? "").Trim()
            });

            Print("Addded/updated inventory item.");
            player.PrintInventory();
        }

        private void RemoveItemMenu()
        {
            string name = GetInput("Enter an item name to remove:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            string amountText = GetInput("Amount to remove (number):");
            double amount = ConvertStringToDouble(amountText, fallback: -1);
            if (amount <= 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            bool ok = player.Inventory.Remove(name, amount);
            if (!ok)
            {
                Print("Remove failed. Check the item name and available amount.");
                return;
            }

            Print("Removed item amount.");
            player.PrintInventory();
        }
    }
}