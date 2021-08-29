using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Model;

namespace Catalog.Api.UnitTest.Infrastructure
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<CatalogEntity> itemsCollection)
        {
            //bool existItem = itemsCollection.Find(p => true).Any();
            // if (!existItem)
            //{
            itemsCollection.InsertManyAsync(GetPreconfiguredCatalogItems());
            //}
        }

        public static int GetSomething()
        {
            return 0;
        }


        public static IEnumerable<CatalogEntity> GetPreconfiguredCatalogItems()
        {
            return new List<CatalogEntity>()
            {
                new CatalogEntity()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "Event A",
                    AvailableStock = 5

                },
                new CatalogEntity()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Name = "Event B",
                    AvailableStock = 10
                },
                new CatalogEntity()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Name = "Event C",
                    AvailableStock = 15
                }
            };
        }
    }
}
