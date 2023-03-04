using Amazon.Runtime.Internal;
using Grpc.Core;
using MongoDB.Driver;
using StockService;
using StockService.Models;

namespace StockService.Services
{
    public class StockServiceGrpc : Stock.StockBase
    {
        private readonly ILogger<StockServiceGrpc> _logger;
        private readonly DatabaseService _dbService;
        public StockServiceGrpc(ILogger<StockServiceGrpc> logger, DatabaseService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }
        public override async Task GetAllProducts(VoidRequest request, IServerStreamWriter<StockServiceProductModel> responseStream, ServerCallContext context)
        {
            var stocks = await _dbService.GetAsync();
            foreach (var stock in stocks)
            {
                await responseStream.WriteAsync(new StockServiceProductModel
                {
                    Id = stock.Id,
                    Name = stock.Name,
                    Quantity = stock.Quantity,
                    ReservedQuantity = stock.ReservedQuantity
                });
            };
        }

        public override async Task<StockServiceResponse> CreateProduct(StockServiceProductModel request, ServerCallContext context)
        {
            StockModel stock = new StockModel
            {
                Name = request.Name,
                Quantity = request.Quantity,
                ReservedQuantity = 0
            };
            await _dbService.CreateAsync(stock);
            return await Task.FromResult(new StockServiceResponse
            {
                Success = true,
            });
        }

        public override async Task<StockServiceResponse> UpdateProduct(StockServiceProductModel request, ServerCallContext context)
        {
            var stock = await _dbService.GetAsync(request.Id);
            if (stock is null)
            {
                return await Task.FromResult(new StockServiceResponse
                {
                    Success = false,
                    Message = "Stock not found"
                });
            }

            StockModel updatedStock = new StockModel
            {
                Name = request.Name,
                Quantity = request.Quantity,
                ReservedQuantity = request.ReservedQuantity
            };
            await _dbService.UpdateAsync(request.Id, updatedStock);
            return await Task.FromResult(new StockServiceResponse
            {
                Success = true,
            });
        }

        public override async Task<StockServiceResponse> DeleteProduct(StockServiceProductId request, ServerCallContext context)
        {
            await _dbService.RemoveAsync(request.Id);
            return await Task.FromResult(new StockServiceResponse
            {
                Success = true,
            });
        }

        public override async Task<StockServiceResponse> ReserveProduct(IAsyncStreamReader<StockServiceReserveProduct> requestStream, ServerCallContext context)
        {
            bool _allProductsSufficient = true;
            string insufficientIds = "";
            List<StockModel> reservedProducts = new List<StockModel>();
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var reservedProduct = requestStream.Current;
                var stock = await _dbService.GetAsync(reservedProduct.Id);
                if (stock is null)
                {
                    return await Task.FromResult(new StockServiceResponse
                    {
                        Success = false,
                        Message = $"Stock {reservedProduct.Id} not found"
                    });
                }

                if (stock.Quantity - stock.ReservedQuantity < reservedProduct.Quantity)
                {
                    _allProductsSufficient = false;
                    insufficientIds += reservedProduct.Id + ";";
                    continue;
                }

                stock.ReservedQuantity += reservedProduct.Quantity;
                reservedProducts.Add(stock);
            }

            if (!_allProductsSufficient)
            {
                return await Task.FromResult(new StockServiceResponse
                {
                    Success = _allProductsSufficient,
                    Message = insufficientIds
                });
            }

            //NOT THE BEST WAY
            reservedProducts.ForEach(async x => await _dbService.UpdateAsync(x.Id, x));

            return await Task.FromResult(new StockServiceResponse
            {
                Success = _allProductsSufficient,
                Message = "Products Reserved"
            });
        }
    }
}