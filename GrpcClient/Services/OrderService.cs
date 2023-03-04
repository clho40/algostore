using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient.Services
{
    public static class OrderService
    {
        private static string OrderServiceEndpoint = "https://localhost:7049";
        public static Order.OrderClient GetClient()
        {
            var OrderChannel = GrpcChannel.ForAddress(OrderServiceEndpoint);
            return new Order.OrderClient(OrderChannel);
        }
    }
}
