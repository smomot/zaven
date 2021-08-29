using Catalog.Api.Model;
using Catalog.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository<CatalogEntity> _repository;
        private readonly ILogger<CatalogController> _logger;
       
        public CatalogController(ICatalogRepository<CatalogEntity> repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogEntity>>> Get()
        {
            _logger.LogInformation("GET called.");
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }



        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogEntity>> Get(string id)
        {
            var item = await _repository.GetItemAsync(id);
            if (item == null)
            {
                _logger.LogError($"NotFound - Item with id: {id}, not found.");
                return NotFound();
            }
            return Ok(item);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogEntity), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogEntity>> Post([FromBody] CatalogEntity item)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"BadRequest - Problem while creating item.");
                return BadRequest(ModelState);
            }
            await _repository.CreateItemAsync(item);
            return CreatedAtRoute("Get", new { id = item.Id }, item);
        }


        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Put([FromBody] CatalogEntity item)
        {
            var updateResult = await _repository.UpdateItemAsync(item);
            if (updateResult)
            {
                return Ok(updateResult);
            }
            _logger.LogError($"NotFound - Problem while updating item with id {item.Id}.");
            return NotFound();
        }

        //[HttpDelete("{id:length(24)}", Name = "DeleteCatalogItem")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogEntity), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string id)
        {
            var deleteResult = await _repository.DeleteItemAsync(id);

            if (deleteResult)
            {
                return Ok(await _repository.DeleteItemAsync(id));
            }
            _logger.LogError($"NotFound - Problem while deleting item with id {id}.");
            return NotFound();

        }


    }
}
