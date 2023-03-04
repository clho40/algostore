using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Services
{
    public class DatabaseService
    {
        private readonly IMongoCollection<OrderModel> _ordersCollection;

        public DatabaseService(IOptions<AlgoStoreDatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _ordersCollection = mongoDB.GetCollection<OrderModel>(dbSettings.Value.OrderCollectionName);
        }

        public async Task<List<OrderModel>> GetAsync() => await _ordersCollection.Find(_ => true).ToListAsync();
        public async Task<OrderModel?> GetAsync(string id) => await _ordersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task CreateAsync(OrderModel newOrder) => await _ordersCollection.InsertOneAsync(newOrder);
        public async Task UpdateAsync(string id, OrderModel updatedOrder) => await _ordersCollection.ReplaceOneAsync(x => x.Id == id, updatedOrder);
        public async Task RemoveAsync(string id) => await _ordersCollection.DeleteOneAsync(x => x.Id == id);


    }
}
