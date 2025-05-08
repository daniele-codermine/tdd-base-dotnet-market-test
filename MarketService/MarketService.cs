
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

	private int GenerateNewId()
	{
		var products = GetAllProducts();
		var lastProduct = products.OrderByDescending(p => p.Id).FirstOrDefault();
		return lastProduct is null ? 0 : lastProduct.Id++;
	}
}
