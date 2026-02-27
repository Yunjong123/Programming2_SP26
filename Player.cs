using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Player : Person
    {
        public Player() : base("Player", 25.00) { }

        public Player(string name, double currency) : base(name, currency) { }
    }
}