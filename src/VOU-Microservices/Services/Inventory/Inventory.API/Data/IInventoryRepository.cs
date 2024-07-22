namespace Inventory.API.Data
{
    public interface IInventoryRepository
    {
        Task<Backpack> GetInventory(string userName, CancellationToken cancellationToken);
        Task<Backpack> StoreInventory(Backpack backpack, CancellationToken cancellationToken);
        Task<bool> DeleteInventory(string userName, CancellationToken cancellationToken);
    }
}
