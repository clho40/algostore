namespace GrpcClient.Models
{
    public class StockModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
