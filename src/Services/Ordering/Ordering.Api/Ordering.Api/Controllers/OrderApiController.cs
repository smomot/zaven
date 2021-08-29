using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Ordering.Api.Infrastructure;
using Ordering.Api.Model;
using Ordering.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ordering")]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderingRepository<OrderingEntity> _repository;
        private readonly ILogger<OrderApiController> _logger;
        private readonly DaprClient _dapr;

        public OrderApiController(IOrderingRepository<OrderingEntity> repository, ILogger<OrderApiController> logger, DaprClient dapr)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dapr = dapr ?? throw new ArgumentNullException(nameof(dapr));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderingEntity>>> Get()
        {
            _logger.LogInformation("GET called.");
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }


        [HttpGet("{id}", Name ="Get")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(OrderingEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderingEntity>> Get(string id)
        {
            var order = await _repository.GetItemAsync(id);
            if (order == null)
            {
                _logger.LogError($"NotFound - Item with id: {id}, not found.");
                return NotFound();
            }
            return Ok(order);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OrderingEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderingEntity>> Post([FromBody] OrderingEntity item)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"BadRequest - Problem while creating item.");
                return BadRequest(ModelState);
            }
            await _repository.CreateItemAsync(item);
            //Publish to catalog item service 
            await _dapr.PublishEventAsync<string>("pubsub", "created", item.CatalogItemId);
            return CreatedAtRoute("Get", new { id = item.Id }, item);
        }

        [HttpDelete("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(OrderingEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Cancel(string orderId)
        {
            var order =  await _repository.GetItemAsync(orderId);
            var cancelResult = await _repository.DeleteItemAsync(orderId);

            if (cancelResult)
            {
                //Publish to catalog item service 
                await _dapr.PublishEventAsync<string>("pubsub", "canceled", order.CatalogItemId);
                return Ok(cancelResult);
            }
            else
            {
                _logger.LogError($"NotFound - Problem while deleting item with id {orderId}.");
                return NotFound();
            }
        }

    }

}
