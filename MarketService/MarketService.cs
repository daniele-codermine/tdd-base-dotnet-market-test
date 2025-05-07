namespace MarketServices;

public class MarketService
{
	public List<Product> GetAllProducts()
	{
		var products = Utilities.ReadJson<Product>("products.json");
		return products;
	}

	public Product GetProductById(int id)
	{
		var products = GetAllProducts();
		return products.FirstOrDefault(p => p.Id == id);
	}
}
