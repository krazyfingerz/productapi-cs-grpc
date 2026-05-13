using ProductAPI.Models;

namespace ProductAPI.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);

    Task<Product?> GetByIdAsync(Guid id);

    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> UpdateAsync(Product product);

    Task<bool> DeleteAsync(Guid id);
}