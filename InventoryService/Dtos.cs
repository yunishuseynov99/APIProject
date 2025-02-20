namespace InventoryService.Dtos
{
    public class Dtos
    {
        public record CatalogItemDto(Guid id, string name, string description);

        public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);

        public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
    }
}
