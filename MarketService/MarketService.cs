namespace MarketServices;

public class MarketService
{

	// Per semplicità/tempo a disposizione il "cart" è qui e in memoria
	private readonly List<Product> _cart;

	public MarketService()
	{
		_cart = new List<Product>(); // Inizializza il carrello come lista vuota
	}

	public List<Product> GetCart()
	{
		return _cart;
	}

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

	public void AddProductToCart(int productId)
	{
		var product = GetProductById(productId);
		if (product == null)
		{
			throw new ProductNotFoundException($"Product with ID {productId} not found in the catalog.");
		}

		_cart.Add(product);
	}

	public void RemoveProductFromCart(int productId)
	{
		var product = _cart.FirstOrDefault(p => p.Id == productId);
		if (product == null)
		{
			throw new ProductNotFoundException($"Product with ID {productId} not found in the cart.");
		}

		_cart.Remove(product);
	}

	public List<(int ProductId, int Quantity, decimal TotalPrice)> GetCartSummary()
	{
		return _cart
			.GroupBy(p => p.Id)
			.Select(group => (
				ProductId: group.Key,
				Quantity: group.Count(),
				TotalPrice: Math.Round(group.Sum(p => p.Price), 2)
			))
			.ToList();
	}

	public decimal GetCartTotal()
	{
	   return _cart.Sum(p => p.Price);
	}

	public decimal ApplyFixedDiscountCoupon(decimal total, decimal couponValue)
	{
		if (total - couponValue < 0)
		{
			throw new InvalidOperationException("Total after discount cannot be negative.");
		}

		return total - couponValue;
	}

	public decimal ApplyPercentageDiscount(decimal total, decimal discountPercentage)
	{
		if (total >= 100)
		{
			var discount = total * (discountPercentage / 100);
			return total - discount;
		}

		return total;
	}

	public decimal ApplyThreeForTwoDiscount()
	{
		var groupedProducts = _cart
			.GroupBy(p => p.Id)
			.Select(group => new
			{
				ProductId = group.Key,
				UnitPrice = group.First().Price,
				Quantity = group.Count()
			});

		decimal total = 0;

		foreach (var product in groupedProducts)
		{
			int groupsOfThree = product.Quantity / 3; // gruppi da 3
			int remainingProducts = product.Quantity % 3; // prodotti rimanenti
			total += (groupsOfThree * 2 * product.UnitPrice) + (remainingProducts * product.UnitPrice);
		}

		return total;
	}

	public decimal ApplyBundleDiscount()
	{
		// Per l'esempio, consideriamo un bundle di 3 prodotti specifici (Banana, Mela, Arancia)
		// In una situazione reale, si dovrebbe fare una gestione più complessa
		var bundleProductIds = new HashSet<int> { 1, 2, 3 }; // Banana, Mela, Arancia
		const decimal bundlePrice = 8m; // Prezzo del bundle

		// Verifica se tutti i prodotti del bundle sono presenti nel carrello
		var cartProductIds = _cart.Select(p => p.Id).ToList();
		if (bundleProductIds.All(id => cartProductIds.Contains(id)))
		{
			// Rimuove i prodotti del bundle dal carrello per evitare doppio conteggio
			foreach (var id in bundleProductIds)
			{
				var productToRemove = _cart.First(p => p.Id == id);
				_cart.Remove(productToRemove); // Non è bello, ma per ora va bene ...
			}

			// Calcola il totale con il prezzo bundle
			return bundlePrice + _cart.Sum(p => p.Price);
		}

		// Nessun bundle applicabile, restituisce il totale normale
		return _cart.Sum(p => p.Price);
	}

		public decimal ApplyBundleDiscount()
	{
		// Definizione del bundle "Macedonia"
		var bundleProductIds = new HashSet<int> { 1, 2, 3 }; // Banana, Mela, Arancia
		const decimal bundlePrice = 8m;

		// Verifica se tutti i prodotti del bundle sono presenti nel carrello
		var cartProductIds = _cart.Select(p => p.Id).ToList();
		if (bundleProductIds.All(id => cartProductIds.Contains(id)))
		{
			// Rimuove i prodotti del bundle dal carrello per evitare doppio conteggio
			foreach (var id in bundleProductIds)
			{
				var productToRemove = _cart.First(p => p.Id == id);
				_cart.Remove(productToRemove);
			}

			// Calcola il totale con il prezzo bundle
			return bundlePrice + _cart.Sum(p => p.Price);
		}

		// Nessun bundle applicabile, restituisce il totale normale
		return _cart.Sum(p => p.Price);
	}
}
