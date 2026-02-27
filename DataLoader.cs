using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace CraftingSystemMonday
{
    public static class DataLoader
    {
        public static string DataDir()
        {
            return Path.Combine(AppContext.BaseDirectory, "Data");
        }

        public static ContentPack LoadContentPack()
        {
            string dir = DataDir();

            string title = Library.ReadTextFileSafe(Path.Combine(dir, "title.txt"), "Craft Application");
            string welcome = Library.ReadTextFileSafe(Path.Combine(dir, "welcome.txt"), "Welcome.");
            string instructions = Library.ReadTextFileSafe(Path.Combine(dir, "instructions.txt"), "");
            string credits = "Credits: HYJ";

            string creditsPath = Path.Combine(dir, "credits.txt");
            if (File.Exists(creditsPath))
            {
                credits = Library.ReadTextFileSafe(creditsPath, credits);
            }

            return new ContentPack
            {
                Title = (title ?? "").Trim(),
                Welcome = (welcome ?? "").Trim(),
                Instructions = (instructions ?? "").Trim(),
                Credits = (credits ?? "").Trim()
            };
        }

        public static List<Recipe> LoadRecipes(string xmlFileName)
        {
            string path = Path.Combine(DataDir(), xmlFileName);
            var recipes = new List<Recipe>();

            if (!File.Exists(path))
            {
                return recipes;
            }

            var doc = new XmlDocument();
            doc.Load(path);

            XmlNodeList nodes = doc.SelectNodes("/Recipes/Recipe");
            if (nodes == null) return recipes;

            foreach (XmlNode n in nodes)
            {
                if (n.Attributes == null) continue;

                var r = new Recipe
                {
                    Name = Attr(n, "name", "Unnamed recipe"),
                    Description = Attr(n, "description", ""),
                    OutputAmount = AttrDouble(n, "outputAmount", 1.0),
                    OutputAmountType = Attr(n, "outputAmountType", "unit(s)"),
                    OutputValueEach = AttrDouble(n, "outputValueEach", 0.0),
                    RequiredItems = new List<Item>()
                };

                XmlNodeList reqs = n.SelectNodes("Requirements/Item");
                if (reqs != null)
                {
                    foreach (XmlNode itemNode in reqs)
                    {
                        var it = new Item
                        {
                            Name = Attr(itemNode, "name", ""),
                            Description = Attr(itemNode, "description", ""),
                            Amount = AttrDouble(itemNode, "amount", 1.0),
                            AmountType = Attr(itemNode, "amountType", "unit(s)"),
                            Value = AttrDouble(itemNode, "value", 0.0)
                        };

                        if (!string.IsNullOrWhiteSpace(it.Name) && it.Amount > 0)
                        {
                            r.RequiredItems.Add(it);
                        }
                    }
                }

                recipes.Add(r);
            }

            return recipes;
        }

        public static Inventory LoadInventory(string xmlFileName)
        {
            string path = Path.Combine(DataDir(), xmlFileName);
            var inv = new Inventory();

            if (!File.Exists(path))
            {
                return inv;
            }

            var doc = new XmlDocument();
            doc.Load(path);

            XmlNodeList nodes = doc.SelectNodes("/Inventory/Item");
            if (nodes == null) return inv;

            foreach (XmlNode n in nodes)
            {
                var it = new Item
                {
                    Name = Attr(n, "name", ""),
                    Description = Attr(n, "description", ""),
                    Amount = AttrDouble(n, "amount", 1.0),
                    AmountType = Attr(n, "amountType", "unit(s)"),
                    Value = AttrDouble(n, "value", 0.0)
                };

                if (!string.IsNullOrWhiteSpace(it.Name) && it.Amount > 0)
                {
                    inv.AddToCollectionByName(it);
                }
            }

            return inv;
        }

        public static void ExportProgress(string xmlFileName, Player player, Trader vendor, List<Recipe> recipes)
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "Exports");
            Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, xmlFileName);

            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("SaveData");
            root.SetAttribute("version", "1");
            doc.AppendChild(root);

            XmlElement p = doc.CreateElement("Player");
            p.SetAttribute("name", player.Name);
            p.SetAttribute("currency", player.Currency.ToString(CultureInfo.InvariantCulture));
            root.AppendChild(p);

            AppendInventory(doc, p, "Inventory", player.Inventory);

            XmlElement v = doc.CreateElement("Vendor");
            v.SetAttribute("name", vendor.Name);
            v.SetAttribute("currency", vendor.Currency.ToString(CultureInfo.InvariantCulture));
            root.AppendChild(v);

            AppendInventory(doc, v, "Inventory", vendor.Inventory);

            XmlElement rs = doc.CreateElement("Recipes");
            root.AppendChild(rs);

            foreach (Recipe r in recipes)
            {
                XmlElement re = doc.CreateElement("Recipe");
                re.SetAttribute("name", r.Name);
                re.SetAttribute("outputAmount", r.OutputAmount.ToString(CultureInfo.InvariantCulture));
                re.SetAttribute("outputAmountType", r.OutputAmountType);
                rs.AppendChild(re);
            }

            doc.Save(path);
        }

        private static void AppendInventory(XmlDocument doc, XmlElement parent, string nodeName, Inventory inv)
        {
            XmlElement invNode = doc.CreateElement(nodeName);
            parent.AppendChild(invNode);

            var list = inv.Items();
            foreach (Item it in list)
            {
                XmlElement item = doc.CreateElement("item");
                item.SetAttribute("name", it.Name);
                item.SetAttribute("description", it.Description ?? "");
                item.SetAttribute("amount", it.Amount.ToString(CultureInfo.InvariantCulture));
                item.SetAttribute("amountType", it.AmountType ?? "unit(s)");
                item.SetAttribute("value", it.Value.ToString(CultureInfo.InvariantCulture));
                invNode.AppendChild(item);
            }
        }

        private static string Attr(XmlNode node, string name, string fallback)
        {
            if (node?.Attributes == null) return fallback;
            XmlAttribute a = node.Attributes[name];
            if (a == null) return fallback;
            return (a.Value ?? "").Trim();
        }

        private static double AttrDouble(XmlNode node, string name, double fallback)
        {
            string s = Attr(node, name, "");
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) return v;
            return fallback;
        }
    }

    public class ContentPack
    {
        public string Title { get; set; } = "Craft Application";
        public string Welcome { get; set; } = "";
        public string Instructions { get; set; } = "";
        public string Credits { get; set; } = "Credits: HYJ";
    }
}