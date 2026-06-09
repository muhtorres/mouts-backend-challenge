using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

/// <summary>
/// Integration tests verifying database operations for SaleRepository.
/// </summary>
public class SaleRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _context.Database.EnsureCreated();
        _repository = new SaleRepository(_context);
    }

    [Fact(DisplayName = "Given valid sale When creating sale Then should persist in database")]
    public async Task CreateAsync_ValidSale_PersistsInDatabase()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SL-999",
            CustomerName = "Alice Smith",
            CustomerId = Guid.NewGuid(),
            BranchName = "North Branch",
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), ProductName = "Product A", Quantity = 5, UnitPrice = 10.0m }
            }
        };

        // Act
        var created = await _repository.CreateAsync(sale);

        // Assert
        Assert.NotNull(created);
        var persisted = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        Assert.NotNull(persisted);
        Assert.Equal("SL-999", persisted.SaleNumber);
        Assert.Single(persisted.Items);
        Assert.Equal("Product A", persisted.Items[0].ProductName);
    }

    [Fact(DisplayName = "Given existing sale When querying by ID Then should load sale with items")]
    public async Task GetByIdAsync_ExistingSale_LoadsWithItems()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SL-100",
            CustomerName = "Bob Smith",
            CustomerId = Guid.NewGuid(),
            BranchName = "South Branch",
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), ProductName = "Product B", Quantity = 2, UnitPrice = 50.0m }
            }
        };
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _repository.GetByIdAsync(sale.Id);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("Bob Smith", loaded.CustomerName);
        Assert.Single(loaded.Items);
        Assert.Equal("Product B", loaded.Items[0].ProductName);
    }

    [Fact(DisplayName = "Given existing sales When listing with filters Then should return matched sales")]
    public async Task ListAsync_WithFiltersAndPagination_ReturnsPaginatedFilteredResults()
    {
        // Arrange
        var sale1 = new Sale { Id = Guid.NewGuid(), SaleNumber = "SL-A", CustomerName = "Jack Daniels", BranchName = "East", Items = new() { new() { Quantity = 2, UnitPrice = 10 } } };
        var sale2 = new Sale { Id = Guid.NewGuid(), SaleNumber = "SL-B", CustomerName = "Johnnie Walker", BranchName = "West", Items = new() { new() { Quantity = 4, UnitPrice = 20 } } };
        await _context.Sales.AddRangeAsync(sale1, sale2);
        await _context.SaveChangesAsync();

        var filters = new Dictionary<string, string>
        {
            { "customerName", "Jack*" }
        };

        // Act
        var (data, totalCount) = await _repository.ListAsync(pageNumber: 1, pageSize: 10, order: "saleNumber desc", filters: filters);

        // Assert
        Assert.Single(data);
        Assert.Equal(1, totalCount);
        Assert.Equal("Jack Daniels", data[0].CustomerName);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
