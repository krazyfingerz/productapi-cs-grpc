using ProductAPI.Services;
using ProductAPI.Caching;
using ProductAPI.Mapping;
using ProductAPI.Repositories;
using ProductAPI.Interceptors;


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support HTTP/2 for gRPC. Docker defaults to HTTP/1.1, build WILL fail.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();       // Interceptor to handle exceptions Globally without leaking internal details
});
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IProductCacheService, ProductCacheService>();
builder.Services.AddSingleton<ExceptionInterceptor>();

var app = builder.Build();

// HTTP request pipeline.
app.MapGrpcService<ProductGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

// Partial class to allow integration testing to access Program.cs internal members
public partial class Program
{
}