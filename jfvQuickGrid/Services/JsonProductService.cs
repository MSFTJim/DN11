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
        finally
        {
            fileLock.Release();
        }

        notificationService.NotifyProductUpdated($"Product '{product.Name}' was updated.");
    }

    private async Task<IReadOnlyList<Product>> ReadProductsAsync()
    {
        await using var stream = File.OpenRead(GetDataFilePath());
        return await JsonSerializer.DeserializeAsync<Product[]>(stream, JsonOptions)
            ?? throw new InvalidDataException("The product data file must contain a JSON array.");
    }

    private string GetDataFilePath()
    {
        return Path.Combine(environment.ContentRootPath, "Data", "products.json");
    }
}
