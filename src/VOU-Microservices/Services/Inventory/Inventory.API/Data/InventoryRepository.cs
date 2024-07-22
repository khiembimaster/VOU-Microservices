namespace Inventory.API.Data
{
    public class InventoryRepository(IDocumentSession session) : IInventoryRepository
    {
        public async Task<bool> DeleteInventory(string userName, CancellationToken cancellationToken)
        {
            session.Delete<Backpack>(userName);
            await session.SaveChangesAsync();
            return true;
        }

        public async Task<Backpack> GetInventory(string userName, CancellationToken cancellationToken)
        {
            var backpack = await session.LoadAsync<Backpack>(userName, cancellationToken);

            return backpack ?? throw new BackpackNotFoundException(userName);
        }

        public async Task<Backpack> StoreInventory(Backpack backpack, CancellationToken cancellationToken)
        {
            session.Store(backpack);
            await session.SaveChangesAsync();
            return backpack;
        }
    }
}
