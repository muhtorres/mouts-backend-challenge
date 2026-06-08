using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a sale by its unique sale number
    /// </summary>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Deletes a sale from the database
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
        {
            return false;
        }

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Lists sales with pagination, sorting, and filtering
    /// </summary>
    public async Task<(List<Sale> Data, int TotalCount)> ListAsync(
        int pageNumber,
        int pageSize,
        string? order,
        Dictionary<string, string>? filters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = _context.Sales.Include(s => s.Items).AsQueryable();

        // 1. Apply filters
        query = ApplyFilters(query, filters);

        // 2. Count total items
        int totalCount = await query.CountAsync(cancellationToken);

        // 3. Apply sorting
        query = ApplySorting(query, order);

        // 4. Apply pagination
        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (data, totalCount);
    }

    private IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, Dictionary<string, string>? filters)
    {
        if (filters == null || filters.Count == 0)
        {
            return query;
        }

        foreach (var filter in filters)
        {
            var key = filter.Key.Trim();
            var val = filter.Value.Trim();
            if (string.IsNullOrEmpty(val)) continue;

            if (key.Equals("customerName", StringComparison.OrdinalIgnoreCase))
            {
                query = ApplyStringFilter(query, s => s.CustomerName, val);
            }
            else if (key.Equals("branchName", StringComparison.OrdinalIgnoreCase))
            {
                query = ApplyStringFilter(query, s => s.BranchName, val);
            }
            else if (key.Equals("saleNumber", StringComparison.OrdinalIgnoreCase))
            {
                query = ApplyStringFilter(query, s => s.SaleNumber, val);
            }
            else if (key.Equals("_minTotalAmount", StringComparison.OrdinalIgnoreCase) && decimal.TryParse(val, out var minAmount))
            {
                query = query.Where(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount) >= minAmount);
            }
            else if (key.Equals("_maxTotalAmount", StringComparison.OrdinalIgnoreCase) && decimal.TryParse(val, out var maxAmount))
            {
                query = query.Where(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount) <= maxAmount);
            }
            else if (key.Equals("_minDate", StringComparison.OrdinalIgnoreCase) && DateTime.TryParse(val, out var minDate))
            {
                query = query.Where(s => s.SaleDate >= minDate);
            }
            else if (key.Equals("_maxDate", StringComparison.OrdinalIgnoreCase) && DateTime.TryParse(val, out var maxDate))
            {
                query = query.Where(s => s.SaleDate <= maxDate);
            }
        }

        return query;
    }

    private IQueryable<Sale> ApplyStringFilter(IQueryable<Sale> query, Expression<Func<Sale, string>> propertySelector, string value)
    {
        var parameter = propertySelector.Parameters[0];
        var member = propertySelector.Body;

        if (value.StartsWith("*") && value.EndsWith("*"))
        {
            var cleanValue = value.Substring(1, value.Length - 2);
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var check = Expression.Call(member, method, Expression.Constant(cleanValue));
            var lambda = Expression.Lambda<Func<Sale, bool>>(check, parameter);
            return query.Where(lambda);
        }
        else if (value.StartsWith("*"))
        {
            var cleanValue = value.Substring(1);
            var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
            var check = Expression.Call(member, method, Expression.Constant(cleanValue));
            var lambda = Expression.Lambda<Func<Sale, bool>>(check, parameter);
            return query.Where(lambda);
        }
        else if (value.EndsWith("*"))
        {
            var cleanValue = value.Substring(0, value.Length - 1);
            var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
            var check = Expression.Call(member, method, Expression.Constant(cleanValue));
            var lambda = Expression.Lambda<Func<Sale, bool>>(check, parameter);
            return query.Where(lambda);
        }
        else
        {
            var check = Expression.Equal(member, Expression.Constant(value));
            var lambda = Expression.Lambda<Func<Sale, bool>>(check, parameter);
            return query.Where(lambda);
        }
    }

    private IQueryable<Sale> ApplySorting(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return query.OrderBy(s => s.CreatedAt);
        }

        var parts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        bool isFirst = true;

        foreach (var part in parts)
        {
            var cleanPart = part.Trim();
            var matches = cleanPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (matches.Length == 0) continue;

            var propertyName = matches[0].Trim();
            bool desc = matches.Length > 1 && matches[1].Trim().Equals("desc", StringComparison.OrdinalIgnoreCase);

            IOrderedQueryable<Sale>? orderedQuery = query as IOrderedQueryable<Sale>;

            if (propertyName.Equals("saleNumber", StringComparison.OrdinalIgnoreCase))
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.SaleNumber) : query.OrderBy(s => s.SaleNumber))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.SaleNumber) : orderedQuery!.ThenBy(s => s.SaleNumber));
            }
            else if (propertyName.Equals("saleDate", StringComparison.OrdinalIgnoreCase))
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.SaleDate) : orderedQuery!.ThenBy(s => s.SaleDate));
            }
            else if (propertyName.Equals("customerName", StringComparison.OrdinalIgnoreCase))
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.CustomerName) : orderedQuery!.ThenBy(s => s.CustomerName));
            }
            else if (propertyName.Equals("branchName", StringComparison.OrdinalIgnoreCase))
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.BranchName) : query.OrderBy(s => s.BranchName))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.BranchName) : orderedQuery!.ThenBy(s => s.BranchName));
            }
            else if (propertyName.Equals("totalAmount", StringComparison.OrdinalIgnoreCase))
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount)) : query.OrderBy(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount)))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount)) : orderedQuery!.ThenBy(s => s.Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount)));
            }
            else
            {
                query = isFirst
                    ? (desc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt))
                    : (desc ? orderedQuery!.ThenByDescending(s => s.CreatedAt) : orderedQuery!.ThenBy(s => s.CreatedAt));
            }

            isFirst = false;
        }

        return query;
    }
}
