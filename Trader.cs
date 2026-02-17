namespace CraftingSystemMonday
{
    public class Trader : Person
    {
        public Trader() : base("Vendr", 0.00)
        {
            foreach (Item it in TestData.TraderStarterItems())
            {
                Inventory.Add(it);
            }
        }

        public Trader(string name, double currency = 0.00) : base(name, currency)
        {
            foreach (Item it in TestData.TraderStarterItems())
            {
                Inventory.Add(it);
            }
        }
    }
}