public class Product
{
	[System.Text.Json.Serialization.JsonPropertyName("id")]
	public int Id { get; set; }
	[System.Text.Json.Serialization.JsonPropertyName("name")]
	public string Name { get; set; }
	[System.Text.Json.Serialization.JsonPropertyName("price")]
	public decimal Price { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName("vatRate")]
	public decimal VatRate { get; set; }
}