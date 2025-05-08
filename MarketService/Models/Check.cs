public class Check
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [System.Text.Json.Serialization.JsonPropertyName("products")]
    public List<CheckProduct> Products { get; set; } = new List<CheckProduct>();

    [System.Text.Json.Serialization.JsonPropertyName("discount")]
    public List<Discount> Discounts { get; set; } = new List<Discount>();




    [System.Text.Json.Serialization.JsonPropertyName("totalPrice")]
    public decimal TotalPrice => Products.Sum(p => p.SoldPrice * p.Quantity);

    [System.Text.Json.Serialization.JsonPropertyName("totalDiscount")]
    public decimal TotalDiscount => Discounts.Sum(d => d.GetDiscount(TotalPrice, Products));

    [System.Text.Json.Serialization.JsonPropertyName("finalPrice")]
    public decimal FinalPrice => TotalPrice - TotalDiscount;
}