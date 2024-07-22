namespace Inventory.API.Models
{
    public class Backpack
    {
        public string UserName { get; set; } = default!;
        public List<BackpackItem> Items { get; set; } = new();
        
        public Backpack(string userName)
        {
            UserName = userName;
        }

        //Required for Mapping
        public Backpack() { }
    }
}
