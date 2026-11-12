namespace jfvQuickGrid.Models;

public sealed record Product(
    int Id,
    string Name,
    string Category,
    decimal Price,
    int Stock,
    string Brand,
    string Sku,
    decimal WeightKg,
    string Description);
