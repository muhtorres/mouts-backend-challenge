using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent
{
    /// <summary>
    /// Gets the cancelled sale details.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCancelledEvent"/> class.
    /// </summary>
    /// <param name="sale">The cancelled sale.</param>
    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
    }
}
