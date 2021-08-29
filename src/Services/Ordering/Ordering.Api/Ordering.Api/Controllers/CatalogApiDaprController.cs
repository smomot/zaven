using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Route("api/v1/orderingcatalog/[action]")]
    public class CatalogApiDaprController : ControllerBase
    {
        private readonly IOrderingRepository<CatalogOrderingEntity> _repository;
        private readonly ILogger<CatalogApiDaprController> _logger;

        public CatalogApiDaprController(IOrderingRepository<CatalogOrderingEntity> repository, ILogger<CatalogApiDaprController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogOrderingEntity>>> Get()
        {
            _logger.LogInformation("GET called.");
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }


        [HttpGet("{id}", Name="GetById")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogOrderingEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogOrderingEntity>> Get(string id)
        {
            var product = await _repository.GetItemAsync(id);
            if (product == null)
            {
                _logger.LogError($"NotFound - Item with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }



        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogOrderingEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogOrderingEntity>> Post([FromBody] CatalogOrderingEntity item)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"BadRequest - Problem while creating item.");
                return BadRequest(ModelState);
            }
            await _repository.CreateItemAsync(item);
            return CreatedAtRoute("GetById", new { id = item.Id }, item);
        }



  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  [ProducesResponseType(typeof(CatalogOrderingEntity), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> IncrementAvailableStock([FromQuery] string id)
  {
     var catalogItem = await _repository.GetItemAsync(id);
     catalogItem.AvailableStock = catalogItem.AvailableStock + 1;

     var updateResult = await _repository.UpdateItemAsync(catalogItem);
     if (updateResult)
     {
        return Ok(updateResult);
     }
        _logger.LogError($"NotFound - Problem while updating item with id {id}.");
        return NotFound();
  }











}

}
