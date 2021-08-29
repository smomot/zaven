using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Model
{
    public class CatalogEntity:BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; }
        public int AvailableStock { get; set; }
    }
}
