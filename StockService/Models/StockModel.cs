using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockService.Models
{
    public class StockModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
