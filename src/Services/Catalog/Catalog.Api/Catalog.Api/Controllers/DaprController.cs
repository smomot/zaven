using Catalog.Api.Model;
using Catalog.Api.Repository;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaprController : ControllerBase
    {
       
        private readonly ILogger<DaprController> _logger;
        private readonly ICatalogRepository<CatalogEntity> _repository;

        public DaprController(ILogger<DaprController> logger, ICatalogRepository<CatalogEntity> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Topic("pubsub", "canceled")]
        [HttpPost("/canceleditems")]
        public async Task<string> ReceiveCancelCatalogItemMessage(string catalogItemId)
        {
            _logger.LogWarning("received ReceiveCancelCatalogItemMessage");
            var catalogItem = await _repository.GetItemAsync(catalogItemId);
            catalogItem.AvailableStock = catalogItem.AvailableStock + 1;
            await _repository.UpdateItemAsync(catalogItem);
            return "received";
        }

        [Topic("pubsub", "created")]
        [HttpPost("/addeditems")]
        public async Task<string> ReceiveAddOrderItemMessage(string catalogItemId)
        {
            _logger.LogWarning("received ReceiveAddOrderItemMessage");
            var catalogItem = await _repository.GetItemAsync(catalogItemId);
            catalogItem.AvailableStock = catalogItem.AvailableStock - 1;
            await _repository.UpdateItemAsync(catalogItem);
            return "received";
        }

    }
}
