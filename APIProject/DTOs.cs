using System;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs
{
    public record itemDto(Guid id, string name, string description, decimal price, DateTimeOffset CreatedDate);

    public record CreateItemDto([Required] string name, string description,[Range(0,(double)decimal.MaxValue)] decimal price);

    public record UpdateItemDto([Required] string name, string description, [Range(0, (double)decimal.MaxValue)] decimal price);
}
