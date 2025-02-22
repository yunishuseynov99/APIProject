using Common;
using InventoryService.Clients;
using InventoryService.Entities;
using Microsoft.AspNetCore.Mvc;
using static InventoryService.Dtos.Dtos;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;
        private readonly IRepository<CatalogItem> _catalogItemsRepository;

        public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            _inventoryItemsRepository = itemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(i => i.UserId == userId);
            var itemIds = inventoryItemEntities.Select(i => i.CatalogItemId);
            var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(i => itemIds.Contains(i.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(ii =>
            {
                var catalogItem = catalogItemEntities.Single(ci => ci.Id == ii.CatalogItemId);
                return ii.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await _inventoryItemsRepository
                .GetAsync(i => i.UserId == grantItemsDto.UserId
                && i.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    UserId = grantItemsDto.UserId,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await _inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok(inventoryItem);
        }
    }
}
