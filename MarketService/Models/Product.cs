public class Product
{
	[System.Text.Json.Serialization.JsonPropertyName("id")]
	public int Id { get; set; }
	[System.Text.Json.Serialization.JsonPropertyName("name")]
	public string Name { get; set; }
	[System.Text.Json.Serialization.JsonPropertyName("price")]
	public decimal Price { get; set; }
}

public class CheckProduct : Product
{
	public CheckProduct()	{
	}

	public CheckProduct(Product product)
	{
		Id = product.Id;
		Name = product.Name;
		Price = product.Price;
		SoldPrice = product.Price;
	}
	[System.Text.Json.Serialization.JsonPropertyName("soldPrice")]
	public decimal SoldPrice { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName("quantity")]
	public int Quantity { get; set; } = 1;
}