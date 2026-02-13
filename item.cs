namespace CraftingSystemMonday
{
    public class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Value { get; set; } = 0.0;
        public double Amount { get; set; } = 1.0;
        public string AmountType { get; set; } = "unit(s)";

        public Item Clone()
        {
            return new Item
            {
                Name = Name,
                Description = Description,
                Value = Value,
                Amount = Amount,
                AmountType = AmountType
            };
        }

        public string Information()
        {
            return $"{Name} = {Amount} {AmountType} | each {Value.ToString("C")}";
        }
    }
}