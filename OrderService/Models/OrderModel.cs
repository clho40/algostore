using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models
{
    public class OrderModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<ReservedProductModel> ReservedStock { get; set; }
    }

    public class ReservedProductModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}
