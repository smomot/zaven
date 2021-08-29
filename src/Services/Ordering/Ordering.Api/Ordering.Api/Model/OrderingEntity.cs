using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Api.Model
{
    [DisplayName("OrderingItems")]
    public class OrderingEntity:BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string CatalogItemId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [BsonElement("CatalogItemName")]
        public string CatalogItemName { get; set; }
        public bool Canceled { get; set; }
    }
}

