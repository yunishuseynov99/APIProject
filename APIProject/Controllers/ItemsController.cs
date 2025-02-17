using CatalogService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private static readonly List<itemDto> _items = new()
        {
            new itemDto(Guid.NewGuid(), "Potion", "Restores a small amount of hp", 5, DateTimeOffset.UtcNow),
            new itemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new itemDto(Guid.NewGuid(), "Brown sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        };
        [HttpGet]
        public IEnumerable<itemDto> Get() 
        {
            return _items;
        }

        [HttpGet("{id}")]
        public itemDto GetById(Guid id) 
        {
            var item = _items.Where(i => i.id == id).SingleOrDefault();
            return item;
        }

        [HttpPost]
        public ActionResult<itemDto> Create([FromBody] CreateItemDto createItemDto) 
        {
            var item = new itemDto(Guid.NewGuid(), createItemDto.name, createItemDto.description, createItemDto.price, DateTimeOffset.UtcNow);
            _items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.id }, item);
        }

        [HttpPut("id")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = _items.Where(i => i.id == id).SingleOrDefault();

            var updatedItem = existingItem with
            {
                name = updateItemDto.name,
                description = updateItemDto.description,
                price = updateItemDto.price,
            };

            var index = _items.FindIndex(i => i.id == id);
            _items[index] = updatedItem;

            return NoContent();
        }

        [HttpDelete("id")]
        public IActionResult Delete(Guid id)
        {
            var index = _items.FindIndex(i => i.id == id);
            _items.RemoveAt(index);

            return NoContent();
        }
    }
}