using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when an individual item in a sale is cancelled.
/// </summary>
public class SaleItemCancelledEvent : INotification
{
    /// <summary>
    /// Gets the parent sale details.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the unique identifier of the cancelled item.
    /// </summary>
    public Guid SaleItemId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemCancelledEvent"/> class.
    /// </summary>
    /// <param name="sale">The parent sale.</param>
    /// <param name="saleItemId">The cancelled sale item ID.</param>
    public SaleItemCancelledEvent(Sale sale, Guid saleItemId)
    {
        Sale = sale;
        SaleItemId = saleItemId;
    }
}
