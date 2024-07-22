namespace Inventory.API.Models
{
    public class BackpackItem
    {
        public int Quantity { get; set; } = default!;
        public Guid ItemId { get; set; } = default!;
        public string ItemName { get; set; } = default!;
    }
}
