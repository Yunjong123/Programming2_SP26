using CraftingSystemMonday;
using static CraftingSystemMonday.Library;

namespace CraftingSystemMonday
{
    public class Player : Person
    {
        public Player() : base("Default Player", 25.00) { }

        public Player(string name, double currency = 25.00) : base(name, currency) { }

        public override void UpdateName()
        {
            string input = GetInput("Enter a new player name:");
            if (!string.IsNullOrWhiteSpace(input))
            {
                Name = input.Trim();
            }
        }
    }
}
