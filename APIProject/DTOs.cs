using System;

namespace CatalogService.DTOs
{
    public record itemDto(Guid id, string name, string description, decimal price, DateTimeOffset CreatedDate);

    public record CreateItemDto(string name, string description, decimal price);

    public record UpdateItemDto(string name, string description, decimal price);
}
