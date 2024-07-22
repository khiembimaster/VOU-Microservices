namespace Inventory.API.Exceptions
{
    public class BackpackNotFoundException : NotFoundException
    {
        public BackpackNotFoundException(string userName) : base("Inventory", userName) { }
    }
}
