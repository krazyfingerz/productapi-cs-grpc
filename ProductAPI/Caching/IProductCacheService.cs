using ProductAPI.Models;

namespace ProductAPI.Caching;

public interface IProductCacheService
{
    Task<Product?> GetAsync(Guid id);

    Task SetAsync(Product product);

    Task RemoveAsync(Guid id);
}