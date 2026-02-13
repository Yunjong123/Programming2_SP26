namespace CraftingSystemMonday
{
    public class Trader : Person
    {
        public Trader() : base("Vendr", 0.00) { }

        public Trader(string name, double currency = 0.00) : base(name, currency) { }
    }
}