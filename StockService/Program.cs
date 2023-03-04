using StockService.Models;
using StockService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.Configure<AlgoStoreDatabaseSettings>(builder.Configuration.GetSection("AlgoStoreDatabase"));
builder.Services.AddSingleton<DatabaseService>();
var app = builder.Build();
app.MapGrpcService<StockServiceGrpc>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
