﻿using System;

namespace Contracts
{
   public record CatalogItemCreated(Guid ItemId, string Name, string Description);
   public record CatalogItemUpdated(Guid ItemId, string Name, string Description);
    public record CatalogItemDeleted(Guid ItemId);
}
