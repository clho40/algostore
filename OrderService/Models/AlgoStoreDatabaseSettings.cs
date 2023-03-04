namespace OrderService.Models
{
    public class AlgoStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set;} = null!;
        public string OrderCollectionName { get; set; } = null!;
    }
}
