using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockService.Models;

namespace StockService.Services
{
    public class DatabaseService
    {
        private readonly IMongoCollection<StockModel> _stocksCollection;

        public DatabaseService(IOptions<AlgoStoreDatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _stocksCollection = mongoDB.GetCollection<StockModel>(dbSettings.Value.StockCollectionName);
        }

        public async Task<List<StockModel>> GetAsync() => await _stocksCollection.Find(_ => true).ToListAsync();
        public async Task<StockModel?> GetAsync(string id) => await _stocksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task CreateAsync(StockModel newStock) => await _stocksCollection.InsertOneAsync(newStock);
        public async Task UpdateAsync(string id, StockModel updatedStock) => await _stocksCollection.ReplaceOneAsync(x => x.Id == id, updatedStock);
        public async Task RemoveAsync(string id) => await _stocksCollection.DeleteOneAsync(x => x.Id == id);


    }
}
