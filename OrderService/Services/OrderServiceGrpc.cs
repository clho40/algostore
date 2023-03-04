using Grpc.Core;
using OrderService;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderServiceGrpc : Order.OrderBase
    {
        private readonly ILogger<OrderServiceGrpc> _logger;
        private readonly DatabaseService _dbService;
        public OrderServiceGrpc(ILogger<OrderServiceGrpc> logger, DatabaseService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        public override async Task<OrderServiceResponse> CreateOrder(OrderServiceOrderModel request, ServerCallContext context)
        {
            var reservedStock = request.Products.Select(x => new ReservedProductModel
            {
                Id = x.Id,
                Quantity = x.Quantity,
            }).ToList();
            OrderModel order = new OrderModel()
            {
                ReservedStock = reservedStock,
            };
            await _dbService.CreateAsync(order);
            return await Task.FromResult(new OrderServiceResponse
            {
                Success = true,
            });
        }
    }
}