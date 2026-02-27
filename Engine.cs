using System.Collections.Generic;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Engine
    {
        private Player player = new Player("Default Player", 25.00);
        private Trader vendor = new Trader("vendor", 0.00);
        private List<Recipe> recipes = new List<Recipe>();
        private ContentPack content = new ContentPack();

        public void Start()
        {
            SetupFromExternalFiles();
            GameLoop();
        }

        public void SetupFromExternalFiles()
        {
            content = DataLoader.LoadContentPack();
            if (!string.IsNullOrWhiteSpace(content.Title))
            {
                Console.Title = content.Title;
            }

            recipes = DataLoader.LoadRecipes("recipes.xml");

            Inventory pInv = DataLoader.LoadInventory("player_inventory.xml");
            Inventory vInv = DataLoader.LoadInventory("vendor_inventory.xml");

            player = new Player(player.Name, player.Currency);
            vendor = new Trader(vendor.Name, vendor.Currency);

            foreach (Item it in pInv.Items())
            {
                player.Inventory.AddToCollectionByName(it);
            }

            foreach (Item it in vInv.Items())
            {
                vendor.Inventory.AddToCollectionByName(it);
            }

            Print("");
            Print(content.Title);
            Print("");
            if (!string.IsNullOrWhiteSpace(content.Welcome)) Print(content.Welcome);
            if (!string.IsNullOrWhiteSpace(content.Instructions))
            {
                Print("");
                Print(content.Instructions);
            }
        }

        private void PrintHud()
        {
            Print("");
            Print($"Player: {player.Name}");
            Print($"Currency: {player.Currency.ToString("C")}");
        }

        public void GameLoop()
        {
            while (true)
            {
                PrintHud();
                Print("");
                Print(MenuText());
                if (!string.IsNullOrWhiteSpace(content.Credits))
                {
                    Print(content.Credits);
                }

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
                        RecipeDetailMenu();
                        break;

                    case 4:
                        CraftMenu();
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
                        TradeMenu();
                        break;

                    case 10:
                        UnitConversionMenu();
                        break;

                    case 11:
                        ExportProgressMenu();
                        break;

                    default:
                        Print("Please enter a valid menu option.");
                        break;
                }
            }
        }

        private string MenuText()
        {
            string output = "Choose an option:";
            output += "\n1. Show Inventory";
            output += "\n2. Show recipes";
            output += "\n3. See recipe details";
            output += "\n4. Craft";
            output += "\n5. Change player name";
            output += "\n6. Search inventory";
            output += "\n7. Add item to inventory";
            output += "\n8. Remove item from inventory";
            output += "\n9. Trade with vendor (buy)";
            output += "\n10. Unit conversion tool";
            output += "\n11. Export progress to XML";
            output += "\n0. Exit";
            return output;
        }

        public void ShowAllRecipes()
        {
            if (recipes.Count == 0)
            {
                Print("No recipes available. Check Data/recipes.xml");
                return;
            }

            Print("Available Recipes:");
            for (int i = 0; i < recipes.Count; i++)
            {
                Print($"  {i + 1}. {recipes[i].Information()}");
            }
        }

        public void RecipeDetailMenu()
        {
            ShowAllRecipes();

            int num = ConvertStringToInteger(GetInput("Enter the recipe number:"));
            if (num < 1 || num > recipes.Count)
            {
                Print($"Please enter a number between 1 and {recipes.Count}.");
                return;
            }

            Print(recipes[num - 1].Details());
        }

        private void CraftMenu()
        {
            ShowAllRecipes();
            if (recipes.Count == 0) return;

            int num = ConvertStringToInteger(GetInput("Enter the recipe number to craft:"));
            if (num < 1 || num > recipes.Count)
            {
                Print($"Please enter a number between 1 and {recipes.Count}.");
                return;
            }

            Recipe recipe = recipes[num - 1];

            if (!CanCraft(recipe, out string reason))
            {
                Print($"Cannot craft {recipe.Name}: {reason}");
                return;
            }

            Item output = CraftItem(recipe);
            player.Inventory.AddToCollectionByName(output);
            Print($"Crafted: {output.Name} x{output.Amount} {output.AmountType}");
        }

        private bool CanCraft(Recipe recipe, out string reason)
        {
            foreach (Item req in recipe.RequiredItems)
            {
                if (!player.Inventory.HasEnough(req.Name, req.Amount))
                {
                    reason = $"missing {req.Name} ({req.Amount} {req.AmountType})";
                    return false;
                }
            }

            reason = "";
            return true;
        }

        private Item CraftItem(Recipe recipe)
        {
            foreach (Item req in recipe.RequiredItems)
            {
                player.Inventory.RemoveAmountByName(req.Name, req.Amount);
            }

            return recipe.CreateOutputItem();
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
            Print($"  {found.Information()}");
        }

        private void AddItemMenu()
        {
            string name = GetInput("Item name:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            double amount = ConvertStringToDouble(GetInput("Amount (number):"), fallback: -1);
            if (amount <= 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            string amountType = GetInput("AMount type (ex: cup(s), tsp(s), unit(s)) [press Enter for unit(s)]:");
            if (string.IsNullOrWhiteSpace(amountType)) amountType = "unit(s)";

            double value = 0.0;
            string valueText = GetInput("Value per item (number) [press Enter for 0]:");
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

            Print("Added/updated item.");
            player.PrintInventory();
        }

        private void RemoveItemMenu()
        {
            string mode = GetInput("Remove By (1) Name+Amount (2) Index? Enter 1 or 2:");
            int m = ConvertStringToInteger(mode, fallback: 1);

            if (m == 2)
            {
                player.PrintInventory();
                int idx1 = ConvertStringToInteger(GetInput("Enter index number to remove (1-based):"), fallback: -1);
                int idx0 = idx1 - 1;

                bool ok = player.Inventory.RemoveFromCollectionByIndexNumber(idx0);
                if (!ok)
                {
                    Print("Remove failed. Check index.");
                    return;
                }

                Print("Removed item by index.");
                player.PrintInventory();
                return;
            }

            string name = GetInput("Enter item name:");
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("Item name cannot be empty.");
                return;
            }

            double amount = ConvertStringToDouble(GetInput("Amount to remove (number):"), fallback: -1);
            if (amount <= 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            bool ok2 = player.Inventory.RemoveAmountByName(name, amount);
            if (!ok2)
            {
                Print("Remove failed. Check name and available amount.");
                return;
            }

            Print("Removed item amount.");
            player.PrintInventory();
        }

        private void TradeMenu()
        {
            PrintHud();
            Print("");
            Print("Vendor Inventory:");
            vendor.PrintInventory();

            if (vendor.Inventory.Count == 0)
            {
                Print("Vendor has nothing to sell.");
                return;
            }

            int idx1 = ConvertStringToInteger(GetInput("Enter item index to buy (1-based, 0 to cancel):"), fallback: 0);
            if (idx1 == 0) return;

            int idx0 = idx1 - 1;
            var list = vendor.Inventory.Items();
            if (idx0 < 0 || idx0 >= list.Count)
            {
                Print("Invalid index.");
                return;
            }

            Item selling = list[idx0];

            double amount = ConvertStringToDouble(GetInput($"Amount to buy (max {selling.Amount}):"), fallback: -1);
            if (amount <= 0)
            {
                Print("Amount must be a positive number.");
                return;
            }

            if (!vendor.Inventory.HasEnough(selling.Name, amount))
            {
                Print("Vendor does not have enough.");
                return;
            }

            double cost = selling.Value * amount;
            if (player.Currency < cost)
            {
                Print($"Not enough currency. Need {cost.ToString("C")}, have {player.Currency.ToString("C")}.");
                return;
            }

            vendor.Inventory.RemoveAmountByName(selling.Name, amount);
            player.Inventory.AddToCollectionByName(new Item
            {
                Name = selling.Name,
                Description = selling.Description,
                Amount = amount,
                AmountType = selling.AmountType,
                Value = selling.Value
            });

            player.Currency -= cost;
            vendor.Currency += cost;

            Print($"Purchased: {selling.Name} x{amount} for {cost.ToString("C")}");
        }

        private void UnitConversionMenu()
        {
            double amount = ConvertStringToDouble(GetInput("Enter amount:"), fallback: double.NaN);
            if (double.IsNaN(amount))
            {
                Print("Invalid amount:");
                return;
            }

            string fromU = GetInput("From unit (tsp/tbsp/cup/pint):");
            string toU = GetInput("To unit (tsp/tbsp/cup/pint):");

            if (!UnitConverter.CanConvert(fromU, toU))
            {
                Print("Unsupported unit(s). Use tsp/tbsp/cup/pint.");
                return;
            }

            double converted = UnitConverter.Convert(amount, fromU, toU);
            Print($"{amount} {fromU} = {converted} {toU}");
        }

        private void ExportProgressMenu()
        {
            DataLoader.ExportProgress("progress_export.xml", player, vendor, recipes);
            Print("Exported: Exports/progress_export.xml");
        }
    }
}