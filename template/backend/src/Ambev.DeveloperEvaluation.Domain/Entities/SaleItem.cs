using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale record.
/// Handles specific business rules for item-level discounting.
/// </summary>
public class SaleItem : BaseEntity
{
    private int _quantity;
    private decimal _unitPrice;

    /// <summary>
    /// Gets or sets the ID of the parent sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the unique external identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name (denormalized).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity sold of this product.
    /// Triggers discount and total calculation when changed.
    /// </summary>
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            CalculateDiscountsAndTotal();
        }
    }

    /// <summary>
    /// Gets or sets the unit price of this product.
    /// Triggers discount and total calculation when changed.
    /// </summary>
    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            _unitPrice = value;
            CalculateDiscountsAndTotal();
        }
    }

    /// <summary>
    /// Gets the discount amount applied to this item.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the total amount for this item (Quantity * UnitPrice - Discount).
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// </summary>
    public SaleItem()
    {
        IsCancelled = false;
    }

    /// <summary>
    /// Cancels this specific sale item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }

    /// <summary>
    /// Calculates discount tier and total amount based on the business rules.
    /// </summary>
    private void CalculateDiscountsAndTotal()
    {
        var rawTotal = _quantity * _unitPrice;

        if (_quantity < 4)
        {
            Discount = 0;
        }
        else if (_quantity >= 4 && _quantity < 10)
        {
            Discount = rawTotal * 0.10m;
        }
        else if (_quantity >= 10 && _quantity <= 20)
        {
            Discount = rawTotal * 0.20m;
        }
        else
        {
            // Restrict quantity > 20 in validator, but handle mathematically here as a fallback
            Discount = rawTotal * 0.20m;
        }

        TotalAmount = rawTotal - Discount;
    }
}
