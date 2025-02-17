using System;

namespace CatalogService.DTOs
{
    public record itemDto(Guid id, string name, string desccription, decimal price, DateTimeOffset CreatedDate);

    public record CreateItemDto(string name, string description, decimal price);

    public record UpdateItem(string name, string Description, decimal price);
}
