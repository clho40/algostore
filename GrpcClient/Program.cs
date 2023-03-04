using Grpc.Net.Client;
using Grpc.Core;
using GrpcClient;
using GrpcClient.Models;
using GrpcClient.Services;


var orderService = OrderService.GetClient();
var stockService = StockService.GetClient();
List<StockModel> stocks = new List<StockModel>();
#region 1. Add mocked data
var input = NextStep("Press 'y' to insert mocked data into MongoDB, or press any keys to skip adding mocked data: ");
if (input == "y")
{
    List<StockServiceProductModel> newStocks = new List<StockServiceProductModel>();
    newStocks.Add(new StockServiceProductModel
    {
        Name = "Durian",
        Quantity = 5,
    });

    newStocks.Add(new StockServiceProductModel
    {
        Name = "Apple",
        Quantity = 5,
    });
    newStocks.Add(new StockServiceProductModel
    {
        Name = "Mango",
        Quantity = 5,
    });
    newStocks.Add(new StockServiceProductModel
    {
        Name = "Avocado",
        Quantity = 5,
    });
    newStocks.Add(new StockServiceProductModel
    {
        Name = "Orange",
        Quantity = 5,
    });
    foreach (var stock in newStocks)
    {
        var result = await stockService.CreateProductAsync(stock);
        Console.WriteLine($"Added {stock.Name} - {result.Success}");
    }
}
Console.WriteLine();
#endregion

#region 2. Query all data
NextStep("Press enter to see all available stock");
await viewStocks();
Console.WriteLine();
#endregion

#region 3. Mock reserve products
NextStep("Press enter to mock stock reservation");
List<StockModel> reservations = new List<StockModel>();
var r0 = stocks[0];
r0.ReservedQuantity = 3;
reservations.Add(r0);

var r1 = stocks[1];
r1.ReservedQuantity = 3;
reservations.Add(r1);

var r2 = stocks[2];
r2.ReservedQuantity = 3;
reservations.Add(r2);

await reserveProduct(reservations);
Console.WriteLine();
#endregion

#region 4. Query all data
NextStep("Press enter to see all available stock");
await viewStocks();
#endregion

Console.ReadLine();

async Task viewStocks()
{
    Console.WriteLine("Stocks: ");
    stocks = new List<StockModel>();
    using (var call = stockService.GetAllProducts(new VoidRequest()))
    {
        while (await call.ResponseStream.MoveNext())
        {
            var product = call.ResponseStream.Current;
            stocks.Add(new StockModel
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                ReservedQuantity = product.ReservedQuantity,
            });
            Console.WriteLine($"{product.Id} = {product.Name} x {product.Quantity} (Reserved: {product.ReservedQuantity})");
        }
    }
}

async Task reserveProduct(List<StockModel> reservations)
{
    using (var call = stockService.ReserveProduct())
    {
        foreach (var reservation in reservations)
        {
            await call.RequestStream.WriteAsync(new StockServiceReserveProduct
            {
                Id = reservation.Id,
                Quantity = reservation.ReservedQuantity,
            });
        }
        await call.RequestStream.CompleteAsync();
        var response = await call;
        if (response.Success)
        {
            Console.WriteLine("Stocks reserved");
            var castedReservedProducts = reservations.Select(x => new OrderServiceReserveProductModel
            {
                Id = x.Id,
                Quantity = x.ReservedQuantity,
            });
            var order = new OrderServiceOrderModel();
            order.Products.Add(castedReservedProducts);
            var orderResponse = await orderService.CreateOrderAsync(order);
            if (orderResponse.Success)
            {
                Console.WriteLine("Order created!");
            }
            else
            {
                Console.WriteLine($"Order is not created - {orderResponse.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Failed. Insufficient stock for {response.Message}");
        }
    }
}

string? NextStep(string msg)
{
    Console.Write(msg);
    return Console.ReadLine();
}
