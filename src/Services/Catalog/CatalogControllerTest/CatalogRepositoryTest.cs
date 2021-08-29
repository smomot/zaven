using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Catalog.Api.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using Catalog.Api.Model;
using Catalog.Api.Repository;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq.Expressions;
using Catalog.Api.UnitTest.Infrastructure;
namespace Catalog.Api.UnitTest
{
    public class CatalogRepositoryTest
    {
        private Mock<ICatalogContext<CatalogEntity>> _mockContext;
        private Mock<IMongoCollection<CatalogEntity>> _mockCollection;
        private CatalogEntity _testElement = null;
        private IEnumerable<CatalogEntity> _testItemsList = null;
        private Mock<IAsyncCursor<CatalogEntity>> _itemCursor = null;

        public CatalogRepositoryTest()
        {
            _mockContext = new Mock<ICatalogContext<CatalogEntity>>();
            _mockCollection = new Mock<IMongoCollection<CatalogEntity>>();

            _testElement = CatalogContextSeed.GetPreconfiguredCatalogItems().First();
            _testItemsList = CatalogContextSeed.GetPreconfiguredCatalogItems();

            //Mock Cursor
            _itemCursor = new Mock<IAsyncCursor<CatalogEntity>>();
            _itemCursor.Setup(_ => _.Current).Returns(_testItemsList);

            _itemCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _itemCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                 .Returns(Task.FromResult(false));

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<CatalogEntity>>(),
             It.IsAny<FindOptions<CatalogEntity, CatalogEntity>>(),
             It.IsAny<CancellationToken>())).ReturnsAsync(_itemCursor.Object);
        }

        #region Read 
        [Fact]
        public async void CatalogItemRepository_GetItem_Success()
        {
            //Arrange

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);

            var CatalogItemRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            //Act
            var result = await CatalogItemRepo.GetItemAsync(_testElement.Id);

            //Assert 

            Assert.NotNull(result);
            Assert.Equal(result.Name, _testElement.Name);
        }

        [Fact]
        public async void CatalogItemRepository_GetAllItems_Valid_Success()
        {
            //Arrange

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);

            var CatalogItemRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            //Act
            var result = await CatalogItemRepo.GetAllAsync();

            //Assert 
            foreach (var item in result)
            {
                Assert.NotNull(item);
            }
        }

        #endregion

        #region Create 

        [Fact]
        public async void CatalogItemRepository_CreateNewItem_Valid_Success()
        {

            //Arrange
            _mockCollection.Setup(op => op.InsertOneAsync(_testElement, null,
            default(CancellationToken))).Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);
            var testRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            //Act
            await testRepo.CreateItemAsync(_testElement);

            //Assert 

            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.InsertOneAsync(_testElement, null, default(CancellationToken)), Times.Once);
        }

        [Fact]
        public async void CatalogItemRepository_CreateNewItem_Null_Failure()
        {
            // Arrange
            _testElement = null;

            //Act 
            var testRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => testRepo.CreateItemAsync(_testElement));
        }



        #endregion

        #region Update
        [Fact]
        public async void CatalogItemRepository_Update_Success()
        {
            //Arange            
            _testElement.Name = "NewValue";
            _mockCollection.Setup(_ => _.ReplaceOneAsync(It.IsAny<FilterDefinition<CatalogEntity>>(), It.IsAny<CatalogEntity>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, _testElement.Id));
            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);

            var CatalogItemRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            //Act
            bool result = await CatalogItemRepo.UpdateItemAsync(_testElement);

            //Assert 
            Assert.True(result);
        }

        [Fact]
        public async void CatalogItemRepository_Update_Invalid_ElementId()
        {
            //Arange
            _testElement.Id = "Bad id";
            _mockCollection.Setup(_ => _.ReplaceOneAsync(It.IsAny<FilterDefinition<CatalogEntity>>(), It.IsAny<CatalogEntity>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 0, _testElement.Id));
            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);

            var CatalogItemRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            //Act
            bool result = await CatalogItemRepo.UpdateItemAsync(_testElement);

            //Assert 
            Assert.False(result);
        }

        #endregion

        #region Delete
        [Fact]
        public async void CatalogItemRepository_DeleteNewItem_Valid_Success()
        {
            //Arrange 

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(_ => _.IsAcknowledged).Returns(true);
            mockDeleteResult.Setup(_ => _.DeletedCount).Returns(1);
            var idToDelete = _testElement.Id;
            Task<DeleteResult> returnTask = Task.FromResult<DeleteResult>(mockDeleteResult.Object);

            _mockCollection.Setup(_ => _.DeleteOneAsync(It.IsAny<FilterDefinition<CatalogEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DeleteResult.Acknowledged(1));
            _mockContext.Setup(c => c.GetCollection<CatalogEntity>(typeof(CatalogEntity).Name)).Returns(_mockCollection.Object);


            //Act 
            var CatalogItemRepo = new CatalogRepository<CatalogEntity>(_mockContext.Object);

            bool result = await CatalogItemRepo.DeleteItemAsync(idToDelete);

            //Assert 

            Assert.True(result);
        }




        #endregion

    }
}
