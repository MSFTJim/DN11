using jfvQuickGrid.Models;

namespace jfvQuickGrid.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task UpdateProductAsync(Product product);
}
