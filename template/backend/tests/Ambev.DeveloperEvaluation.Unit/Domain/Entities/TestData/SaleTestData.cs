using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating mock test data for Sales and SaleItems using the Bogus library.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => decimal.Parse(f.Commerce.Price(1, 100)))
        .RuleFor(i => i.IsCancelled, false);

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SL-{f.Date.Recent().ToString("yyyyMMdd")}-{f.Random.AlphaNumeric(4).ToUpper()}")

        .RuleFor(s => s.SaleDate, f => f.Date.Recent(5))
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Address.City())
        .RuleFor(s => s.IsCancelled, false);

    /// <summary>
    /// Generates a valid SaleItem entity.
    /// </summary>
    public static SaleItem GenerateValidItem()
    {
        var item = SaleItemFaker.Generate();
        return item;
    }

    /// <summary>
    /// Generates a valid SaleItem entity with a custom quantity and unit price.
    /// </summary>
    public static SaleItem GenerateItem(int quantity, decimal unitPrice)
    {
        var item = SaleItemFaker.Generate();
        item.Quantity = quantity;
        item.UnitPrice = unitPrice;
        return item;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specified number of items.
    /// </summary>
    public static Sale GenerateValidSale(int itemCount = 2)
    {
        var sale = SaleFaker.Generate();
        sale.Items = SaleItemFaker.Generate(itemCount);
        foreach (var item in sale.Items)
        {
            item.SaleId = sale.Id;
        }
        return sale;
    }
}
