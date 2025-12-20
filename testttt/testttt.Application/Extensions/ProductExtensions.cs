using testttt.Domain.Entities;

namespace testttt.Application.Extensions;

public static class ProductExtensions
{
    /// <summary>
    /// Calculates the actual price of a product considering active discounts.
    /// Returns the discounted price if discount is active, otherwise returns the regular price.
    /// </summary>
    public static decimal GetDiscountedPrice(this Product product)
    {
        if (!product.DiscountPercentage.HasValue || product.DiscountPercentage.Value <= 0)
        {
            return product.Price;
        }

        var now = DateTime.UtcNow;
        var isDiscountActive = (!product.DiscountStartDate.HasValue || product.DiscountStartDate.Value <= now) &&
                               (!product.DiscountEndDate.HasValue || product.DiscountEndDate.Value >= now);

        if (!isDiscountActive)
        {
            return product.Price;
        }

        var discountAmount = product.Price * (product.DiscountPercentage.Value / 100m);
        return product.Price - discountAmount;
    }

    /// <summary>
    /// Checks if a product has an active discount at the current time.
    /// </summary>
    public static bool HasActiveDiscount(this Product product)
    {
        if (!product.DiscountPercentage.HasValue || product.DiscountPercentage.Value <= 0)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        return (!product.DiscountStartDate.HasValue || product.DiscountStartDate.Value <= now) &&
               (!product.DiscountEndDate.HasValue || product.DiscountEndDate.Value >= now);
    }
}

