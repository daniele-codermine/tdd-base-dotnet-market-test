namespace MarketServices;

public class MarketService
{
	public List<Product> GetAllProducts()
	{
		var products = Utilities.ReadJson<Product>("products.json");
		return products;
	}

	public List<Check> GetAllChecks()
	{
		var checks = Utilities.ReadJson<Check>("checks.json");
		return checks;
	}

	public Product GetProductById(int id)
	{
		var products = GetAllProducts();
		return products.FirstOrDefault(p => p.Id == id);
	}

	public Check CreateCheck(List<CheckProduct> products)
	{
		var check = new Check
		{
			Products = products ?? new List<CheckProduct>(),
		};

		var checks = GetAllChecks();
		checks.Add(check);

		Utilities.WriteJson("checks.json", checks);

		return check;
	}

	public Check RemoveProductFromCheck(Guid checkId, int productId)
	{
		var checks = GetAllChecks();
		var check = checks.FirstOrDefault(c => c.Id == checkId);
		if (check != null)
		{
			var productToRemove = check.Products.FirstOrDefault(p => p.Id == productId);
			if (productToRemove != null)
			{
				check.Products.Remove(productToRemove);
				Utilities.WriteJson("checks.json", checks);
			}
			else
			{
				throw new Exception($"Product with ID {productId} not found in the check.");
			}
		}
		else
		{
			throw new Exception($"Check with ID {checkId} not found.");
		}

		return check;
	}

	public Check UpdateProductInCheck(Guid checkId, CheckProduct updatedProduct)
	{
		var checks = GetAllChecks();
		var check = checks.FirstOrDefault(c => c.Id == checkId);
		if (check != null)
		{
			var productToUpdate = check.Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
			if (productToUpdate != null)
			{
				productToUpdate.Quantity = updatedProduct.Quantity;
				productToUpdate.SoldPrice = updatedProduct.SoldPrice;
				Utilities.WriteJson("checks.json", checks);
			}
			else
			{
				throw new Exception($"Product with ID {updatedProduct.Id} not found in the check.");
			}
		}
		else
		{
			throw new Exception($"Check with ID {checkId} not found.");
		}

		return check;
	}

	public Check ApplyDiscount(Guid checkId, Discount discount)
	{
		var checks = GetAllChecks();
		var check = checks.FirstOrDefault(c => c.Id == checkId);
		if (check != null)
		{
			check.Discounts.Add(discount);
			Utilities.WriteJson("checks.json", checks);
		}
		else
		{
			throw new Exception($"Check with ID {checkId} not found.");
		}

		return check;
	}

	public void DeleteCheck(Guid id)
	{
		var checks = GetAllChecks();
		var checkToDelete = checks.FirstOrDefault(c => c.Id == id);
		if (checkToDelete != null)
		{
			checks.Remove(checkToDelete);
			Utilities.WriteJson("checks.json", checks);
		}
		else
		{
			throw new Exception($"Check with ID {id} not found.");
		}
	}
}
