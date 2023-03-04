using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient.Services
{
    public static class StockService
    {
        private static string StockServiceEndpoint = "https://localhost:7214";
        public static Stock.StockClient GetClient()
        {
            var stockChannel = GrpcChannel.ForAddress(StockServiceEndpoint);
            return new Stock.StockClient(stockChannel);
        }
    }
}
