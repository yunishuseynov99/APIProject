using System;

namespace CatalogService.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}