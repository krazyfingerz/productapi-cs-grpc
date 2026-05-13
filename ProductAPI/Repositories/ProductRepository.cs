using System.Collections.Concurrent;
using ProductAPI.Models;

namespace ProductAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<Product> CreateAsync(Product product)
    {
        _products[product.Id] = product;

        return Task.FromResult(product);
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _products.TryGetValue(id, out var product);

        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.Values.AsEnumerable());
    }

    public Task<Product?> UpdateAsync(Product product)
    {
        if (!_products.ContainsKey(product.Id))
        {
            return Task.FromResult<Product?>(null);
        }

        _products[product.Id] = product;

        return Task.FromResult<Product?>(product);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _products.TryRemove(id, out _);

        return Task.FromResult(removed);
    }
}