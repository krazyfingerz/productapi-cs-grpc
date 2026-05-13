using ProductAPI.Services;
using ProductAPI.Caching;
using ProductAPI.Mapping;
using ProductAPI.Repositories;
using ProductAPI.Interceptors;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Configure the HTTP request pipeline.
app.MapGrpcService<ProductGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program
{
}