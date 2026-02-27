namespace CraftingSystemMonday
{
    public class Trader : Person
    {
        public Trader() : base("Vendor", 0.00) { }

        public Trader(string name, double currency) : base(name, currency) { }
    }
}