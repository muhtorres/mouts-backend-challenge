namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Result returned after cancelling a sale item.
/// </summary>
public class CancelSaleItemResult
{
    public bool Success { get; set; }
    public Guid ItemId { get; set; }
    public bool IsCancelled { get; set; }
}
