using Catalog.Api.UnitTest.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Model;
using Catalog.Api.Repository;
using Xunit;

namespace Catalog.Api.UnitTest
{

    public class CatalogControllerTest
    {
        Mock<ICatalogRepository<CatalogEntity>> stubRepository;
        Mock<ILogger<CatalogController>> stubLoggService;
  
        IEnumerable<CatalogEntity> dataStore;
        CatalogController sut;

        public CatalogControllerTest()
        {
            stubRepository = new Mock<ICatalogRepository<CatalogEntity>>();
            stubLoggService = new Mock<ILogger<CatalogController>>();
            dataStore = CatalogContextSeed.GetPreconfiguredCatalogItems();
      
            sut = new CatalogController(stubRepository.Object, stubLoggService.Object);
        }

        #region Get



        [Fact]
        public async Task Get_Returns_OkObjectResult()
        {
            //Arrange

            stubRepository.Setup(s => s.GetAllAsync()).Returns(Task.FromResult(dataStore));

            //Act 
            var result = await sut.Get();
            //Assert        
            Assert.IsType<OkObjectResult>(result.Result);
        }


        [Fact]
        public async Task Get_Returns_All_Items()
        {
            //Arrange
            stubRepository.Setup(s => s.GetAllAsync()).Returns(Task.FromResult(dataStore));
            //Act 
            var result = await sut.Get();
            var okObjectResult = result.Result as OkObjectResult;

            //Assert
            //
            var items = Assert.IsType<List<CatalogEntity>>(okObjectResult.Value);
            Assert.NotNull(items);
        }



        [Fact]
        public async Task Get_Returns_Null()
        {
            //Arrange
            IEnumerable<CatalogEntity> _dataStore = null;
            stubRepository.Setup(s => s.GetAllAsync()).Returns(Task.FromResult(_dataStore));
            //Act 
            var result = await sut.Get();
            var okObjectResult = result.Result as OkObjectResult;

            //Assert
            //
            Assert.Null(okObjectResult.Value);
        }

        [Fact]
        public async Task Get_By_Id_Returns_NotFoundResult()
        {
            //Arrange
            IEnumerable<CatalogEntity> _dataStore = null;
            CatalogEntity elemeToFind = dataStore.First();
            stubRepository.Setup(s => s.GetAllAsync()).Returns(Task.FromResult(_dataStore));
            //Act 
            var result = await sut.Get(elemeToFind.Id);


            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task Get_By_Id_ExistingIdPassed_Returns_OkResult()
        {
            CatalogEntity itemToFind = dataStore.First();
            stubRepository.Setup(s => s.GetItemAsync(itemToFind.Id)).Returns(Task.FromResult(itemToFind));
            //Act 
            var result = await sut.Get(itemToFind.Id);

            //Assert
            var okFoundResult = Assert.IsType<OkObjectResult>(result.Result);
            var itemFound = Assert.IsType<CatalogEntity>(okFoundResult.Value);
            Assert.Equal(itemToFind.Id, itemFound.Id);
        }





        #endregion

        #region Post

        [Fact]
        public async Task Add_InvalidObjectPassed_Returns_BadRequest()
        {
            // Arrange
            var nameMissingItem = new CatalogEntity()
            {
                Name = "Event D",
                AvailableStock = 15
            };
            sut.ModelState.AddModelError("Id", "Required");
            // Act
            var badResponse = await sut.Post(nameMissingItem);
            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse.Result);
        }

        [Fact]
        public async Task Add_Returns_CreatedAtRouteResult()
        {
            // Arrange
            var itemToCreate = new CatalogEntity()
            {
                Id = "xxx",
                Name = "Event D",
                AvailableStock = 15
            };
            // Act
            var createdResponse = await sut.Post(itemToCreate);
            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(createdResponse.Result);
            var itemReturned = Assert.IsType<CatalogEntity>(createdAtRouteResult.Value);
            Assert.NotNull(itemReturned);
        }


        #endregion

        #region Put 

        [Fact]
        public async Task Update_NotExistingId_Returns_NotFoundResponse()
        {
            // Arrange
            var itemToUpdate = new CatalogEntity()
            {
                Id = "xxx",
                Name = "Event D",
                AvailableStock = 15
            };
            stubRepository.Setup(s => s.UpdateItemAsync(itemToUpdate)).Returns(Task.FromResult(false));
            var sut = new CatalogController(stubRepository.Object, stubLoggService.Object);
            // Act
            var badResponse = await sut.Put(itemToUpdate);
            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }

        [Fact]
        public async Task Update_Returns_OkObjectResult()
        {
            // Arrange
            var itemToUpdate = new CatalogEntity()
            {
                Id = "xxx",
                Name = "Event D",
                AvailableStock = 15
            };
            stubRepository.Setup(s => s.UpdateItemAsync(itemToUpdate)).Returns(Task.FromResult(true));
            var sut = new CatalogController(stubRepository.Object, stubLoggService.Object);
            // Act
            var badResponse = await sut.Put(itemToUpdate);
            // Assert
            Assert.IsType<OkObjectResult>(badResponse);
        }


        #endregion

        #region Delete
        [Fact]
        public async Task Remove_NotExistingId_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistiId = "XXX";
            stubRepository.Setup(s => s.DeleteItemAsync(notExistiId)).Returns(Task.FromResult(false));
            var sut = new CatalogController(stubRepository.Object, stubLoggService.Object);
            // Act
            var badResponse = await sut.Delete(notExistiId);
            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }
        [Fact]
        public async Task Remove_ExistingIdPassed_ReturnsOkResult()
        {
            // Arrange
            CatalogEntity itemToFind = dataStore.First();
            stubRepository.Setup(s => s.DeleteItemAsync(itemToFind.Id)).Returns(Task.FromResult(true));
            var sut = new CatalogController(stubRepository.Object, stubLoggService.Object);
            // Act
            var okResponse = await sut.Delete(itemToFind.Id);
            // Assert
            Assert.IsType<OkObjectResult>(okResponse);
        }
        #endregion


    }



}
