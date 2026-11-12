using System.Text.Json;
using jfvQuickGrid.Models;

namespace jfvQuickGrid.Services;

public sealed class JsonProductService(
    IWebHostEnvironment environment,
    NotificationService notificationService) : IProductService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly SemaphoreSlim fileLock = new(1, 1);

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        await fileLock.WaitAsync();
        try
        {
            return await ReadProductsAsync();
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        Product createdProduct;
        await fileLock.WaitAsync();
        try
        {
            var products = (await ReadProductsAsync()).ToList();
            var highestId = products.Count == 0 ? 0 : products.Max(existing => existing.Id);
            if (highestId == int.MaxValue)
            {
                throw new InvalidOperationException("No product IDs are available.");
            }

            var nextId = Math.Max(0, highestId) + 1;
            createdProduct = product with { Id = nextId };
            products.Add(createdProduct);
            await WriteProductsAsync(products);
        }
        finally
        {
            fileLock.Release();
        }

        notificationService.NotifyProductChanged($"Product '{createdProduct.Name}' was created.");
        return createdProduct;
    }

    public async Task UpdateProductAsync(Product product)
    {
        await fileLock.WaitAsync();
        try
        {
            var products = (await ReadProductsAsync()).ToList();
            var productIndex = products.FindIndex(existing => existing.Id == product.Id);
            if (productIndex < 0)
            {
                throw new InvalidOperationException($"Product with ID {product.Id} was not found.");
            }

            products[productIndex] = product;
            await WriteProductsAsync(products);
        }
        finally
        {
            fileLock.Release();
        }

        notificationService.NotifyProductChanged($"Product '{product.Name}' was updated.");
    }

    public async Task DeleteProductAsync(int productId)
    {
        Product deletedProduct;
        await fileLock.WaitAsync();
        try
        {
            var products = (await ReadProductsAsync()).ToList();
            var productIndex = products.FindIndex(existing => existing.Id == productId);
            if (productIndex < 0)
            {
                throw new InvalidOperationException($"Product with ID {productId} was not found.");
            }

            deletedProduct = products[productIndex];
            products.RemoveAt(productIndex);
            await WriteProductsAsync(products);
        }
        finally
        {
            fileLock.Release();
        }

        notificationService.NotifyProductChanged($"Product '{deletedProduct.Name}' was deleted.");
    }

    private async Task<IReadOnlyList<Product>> ReadProductsAsync()
    {
        await using var stream = File.OpenRead(GetDataFilePath());
        return await JsonSerializer.DeserializeAsync<Product[]>(stream, JsonOptions)
            ?? throw new InvalidDataException("The product data file must contain a JSON array.");
    }

    private async Task WriteProductsAsync(List<Product> products)
    {
        var path = GetDataFilePath();
        var temporaryPath = $"{path}.tmp";
        try
        {
            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, products, JsonOptions);
            }

            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private string GetDataFilePath()
    {
        return Path.Combine(environment.ContentRootPath, "Data", "products.json");
    }
}
