using Microsoft.Extensions.Caching.Memory;
using ProductAPI.Models;

namespace ProductAPI.Caching;

public class ProductCacheService : IProductCacheService
{
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(5);

    public ProductCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<Product?> GetAsync(Guid id)
    {
        _cache.TryGetValue(id, out Product? product);

        return Task.FromResult(product);
    }

    public Task SetAsync(Product product)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };

        _cache.Set(product.Id, product, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid id)
    {
        _cache.Remove(id);

        return Task.CompletedTask;
    }
}