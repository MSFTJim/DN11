using jfvQuickGrid.Models;

namespace jfvQuickGrid.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
}
