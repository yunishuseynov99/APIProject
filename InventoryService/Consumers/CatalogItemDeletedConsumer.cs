using Common;
using Contracts;
using InventoryService.Entities;
using MassTransit;

namespace InventoryService.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> _repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item = await _repository.GetAsync(message.ItemId);

            if (item == null)
            {
                return;
            }
            else
            {

                await _repository.RemoveAsync(message.ItemId);
            }
        }
    }
}
