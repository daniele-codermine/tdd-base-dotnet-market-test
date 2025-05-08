
using System.Security.Authentication;

namespace MarketServices;

public class CarriageService
{
	public List<CarriageItem> _carriageItemsList;
	public MarketService _mkService;

	public CarriageService(MarketService mkService)
	{
		_carriageItemsList = new List<CarriageItem>();
		_mkService = mkService;
	}

    public CarriageItem AddItem(int idProduct, int qta)
    {
		var product = _mkService.GetProductById(idProduct);
		if (product is null)
			throw new ArgumentException("Product not found on db");
		
		if (qta < 1)
			throw new ArgumentException("Quantity most be > 1");
		
		var carriageItem = new CarriageItem();
		carriageItem.Id = _carriageItemsList.Count() + 1;
		carriageItem.IdProduct = product.Id;
		carriageItem.Qta = qta;
		carriageItem.Price = product.Price * qta;
		_carriageItemsList.Add(carriageItem);
		return carriageItem;
    }
	

    public bool RemoveItemById(int itemId)
    {
		var item = GetItemById(itemId);

		if (item is null)
			throw new ArgumentException($"Item {itemId} does not exist");

		_carriageItemsList.Remove(item);
		return true;
    }

	public CarriageItem? GetItemById(int itemId)
	{
		return _carriageItemsList.FirstOrDefault(ci => ci.Id == itemId);
	}

    public CarriageItem UpdateQtaItem(int itemId, int newQta)
    {
        var item = GetItemById(itemId);

		if (item is null)
			throw new ArgumentException($"Item {itemId} does not exist");

		var index = _carriageItemsList.IndexOf(item);
		_carriageItemsList[index].Qta = newQta;
		_carriageItemsList[index].Price = CalcPrice(item.IdProduct, newQta);
		return _carriageItemsList[index];
    }

	private decimal CalcPrice(int idProduct, int qta)
	{
		var product = _mkService.GetProductById(idProduct);
		return product.Price * qta;
	}

	public decimal GetTotal()
	{
		return _carriageItemsList.Sum(ci => ci.Price);
	}

	public decimal GetTotalWithDiscount(decimal discount)
	{
		return GetTotal() - discount;
	}

    public decimal GetDiscount(decimal limit, decimal discountPerc)
    {
		var total = GetTotal();
		if (total < limit)
			return 0;
		
		return total * discountPerc / 100;
    }
}
