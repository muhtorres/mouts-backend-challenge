using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the Sale and SaleItem entities, focusing on discount rules and validations.
/// </summary>
public class SaleTests
{
    [Theory(DisplayName = "No discount should be applied for quantities below 4")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityLessThanFour_When_Calculated_Then_DiscountShouldBeZero(int quantity)
    {
        // Arrange
        decimal unitPrice = 10.0m;

        // Act
        var item = SaleTestData.GenerateItem(quantity, unitPrice);

        // Assert
        Assert.Equal(0, item.Discount);
        Assert.Equal(quantity * unitPrice, item.TotalAmount);
    }

    [Theory(DisplayName = "10% discount should be applied for quantities between 4 and 9")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_Calculated_Then_DiscountShouldBeTenPercent(int quantity)
    {
        // Arrange
        decimal unitPrice = 10.0m;
        decimal expectedDiscount = quantity * unitPrice * 0.10m;
        decimal expectedTotal = (quantity * unitPrice) - expectedDiscount;

        // Act
        var item = SaleTestData.GenerateItem(quantity, unitPrice);

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Theory(DisplayName = "20% discount should be applied for quantities between 10 and 20")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetweenTenAndTwenty_When_Calculated_Then_DiscountShouldBeTwentyPercent(int quantity)
    {
        // Arrange
        decimal unitPrice = 10.0m;
        decimal expectedDiscount = quantity * unitPrice * 0.20m;
        decimal expectedTotal = (quantity * unitPrice) - expectedDiscount;

        // Act
        var item = SaleTestData.GenerateItem(quantity, unitPrice);

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Fact(DisplayName = "Sale total amount should be the sum of non-cancelled items")]
    public void Given_SaleWithItems_When_TotalCalculated_Then_TotalShouldBeSumOfActiveItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(3);
        var item1 = sale.Items[0];
        var item2 = sale.Items[1];
        var item3 = sale.Items[2];

        // Act & Assert
        var expectedTotal = item1.TotalAmount + item2.TotalAmount + item3.TotalAmount;
        Assert.Equal(expectedTotal, sale.TotalAmount);

        // Cancel item2
        sale.CancelItem(item2.Id);

        // New expected total should not include item2
        var expectedNewTotal = item1.TotalAmount + item3.TotalAmount;
        Assert.Equal(expectedNewTotal, sale.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling a sale should set total amount to 0 and cancel all items")]
    public void Given_Sale_When_Cancelled_Then_TotalShouldBeZeroAndAllItemsCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(2);

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.Equal(0, sale.TotalAmount);
        Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
    }

    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(2);

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Validation should fail when quantity is above 20")]
    public void Given_SaleWithQuantityAboveTwenty_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(1);
        sale.Items[0].Quantity = 21; // Rule limit is max 20

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, err => err.Detail.Contains("above 20") || err.Detail.Contains("20"));
    }

    [Fact(DisplayName = "Validation should fail when sale has no items")]
    public void Given_SaleWithNoItems_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(0);
        sale.Items.Clear();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, err => err.Detail.Contains("at least one item"));
    }
}
