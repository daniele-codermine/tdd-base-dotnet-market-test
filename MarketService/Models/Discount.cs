public abstract class BaseDiscount
{
    public virtual decimal GetDiscount(decimal totalPrice, List<CheckProduct> products)
    {
        return 0;
    }
}

public class Discount : BaseDiscount
{
    public decimal Threshold { get; set; } = 0;
    public bool IsThresholdReached(decimal totalPrice)
    {
        return totalPrice >= Threshold;
    }

    public override decimal GetDiscount(decimal totalPrice, List<CheckProduct> products)
    {
        return 0;
    }
}

public class FixedDiscount : Discount
{
    public decimal Amount { get; set; } = 0;

    public override decimal GetDiscount(decimal totalPrice, List<CheckProduct> products){
        if (IsThresholdReached(totalPrice))
        {
            return Amount;
        }
        return 0;
    }
}

public class PercentageDiscount : Discount
{
    public decimal Percentage { get; set; } = 0;

    public override decimal GetDiscount(decimal totalPrice, List<CheckProduct> products)
    {
        if (IsThresholdReached(totalPrice))
        {
            return totalPrice * Percentage / 100;
        }
        return 0;
    }
}

public class MultiBuyDiscount : Discount
{
    public (int productId, int thresholdQty, int discountQty) MultiBuy { get; set; }

    public override decimal GetDiscount(decimal totalPrice, List<CheckProduct> products)
    {
        if (IsThresholdReached(totalPrice))
        {
            decimal discount = 0;
            var product = products.FirstOrDefault(p => p.Id == MultiBuy.productId);
                if (product != null)
                {
                    int rem = product.Quantity / MultiBuy.thresholdQty;
                    if(rem > 0)
                        discount += rem * MultiBuy.discountQty * product.SoldPrice;
                }
            return discount;
        }
        return 0;
    }
}

public class BundleDiscount : Discount
{
    public (List<int> productIds, decimal bundlePrice) Bundle { get; set; }

    public override decimal GetDiscount(decimal totalPrice, List<CheckProduct> products)
    {
        if (IsThresholdReached(totalPrice))
        {
            var bundleProducts = products.Where(p => Bundle.productIds.Contains(p.Id)).ToList();
            if (bundleProducts.Count == Bundle.productIds.Count)
            {
                decimal bundleTotalPrice = bundleProducts.Sum(p => p.SoldPrice * p.Quantity);
                var minQty = bundleProducts.Min(p => p.Quantity);
                return  bundleProducts.Sum(p => p.SoldPrice * minQty) - (Bundle.bundlePrice * minQty);
            }
        }
        return 0;
    }
}