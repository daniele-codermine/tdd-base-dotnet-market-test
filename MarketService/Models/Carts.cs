using System.Text;

namespace MarketServices.Models;

public class Cart
{
    private readonly Dictionary<int, (Product product, int quantity)> _items = new();
    private decimal _total;

    public void AddProduct(Product product, int quantity)
    {
        if (_items.ContainsKey(product.Id))
            _items[product.Id] = (product, _items[product.Id].quantity + quantity);
        else
            _items[product.Id] = (product, quantity);

        UpdateTotal();
    }

    public void RemoveProduct(Product product)
    {
        if (_items.ContainsKey(product.Id))
            _items.Remove(product.Id);

        UpdateTotal();
    }

    public void UpdateProductQuantity(Product product, int newQuantity)
    {
        if (_items.ContainsKey(product.Id))
            _items[product.Id] = (product, newQuantity);

        UpdateTotal();
    }

    public decimal Total => _total;

    public decimal TotalWithVAT => Math.Round(_items.Sum(i =>
    {
        var product = i.Value.product;
        var qty = i.Value.quantity;
        var priceWithVat = product.Price * (1 + Math.Round(product.VatRate, 2));
        return priceWithVat * qty;
    }), 2);

    private void UpdateTotal()
    {
        _total = Math.Round(_items.Sum(i => i.Value.product.Price * i.Value.quantity), 2);
    }

    public void ApplyFixedDiscount(decimal discountAmount)
    {
        if (_total > discountAmount)
        {
            _total -= discountAmount;
        }
        else
        {
            _total = 0;
        }
    }

    public void ApplyPercentageDiscount(decimal percentage)
    {
        if (_total >= 100)
        {
            _total -= _total * (percentage / 100);
        }
    }

    public void Apply3x2Discount()
    {
        foreach (var item in _items)
        {
            // Se ci sono almeno 3 prodotti, il terzo è gratuito
            if (item.Value.quantity >= 3)
            {
                int freeItems = item.Value.quantity / 3; // Ogni gruppo di 3 prodotti ha 1 gratuito
                decimal discount = freeItems * item.Value.product.Price;
                _total -= discount;
            }
        }
    }
    public void ApplyBundleDiscount(string bundleName, decimal bundlePrice)
    {
        var bundleProducts = new List<int> { 1, 2, 3 }; // ID dei prodotti coinvolti nel bundle, il passo successivo sarebbe fare un catalogo dei bundle

        if (bundleProducts.All(id => _items.ContainsKey(id)))
        {
            _total = bundlePrice;
        }
    }

    public string GenerateReceipt()
    {
        var receipt = new StringBuilder();
        receipt.AppendLine("Scontrino");
        receipt.AppendLine("---------");

        decimal total = 0;

        foreach (var item in _items)
        {
            var product = item.Value.product;
            var quantity = item.Value.quantity;
            var priceWithVat = Math.Round(product.Price * (1 + product.VatRate), 2);
            var totalProductPrice = Math.Round(priceWithVat * quantity, 2);
            var vatAmount = totalProductPrice - Math.Round(product.Price * quantity, 2);

            receipt.AppendLine($"{product.Name} - ");
            receipt.AppendLine($"  {quantity} x {product.Price}€ = {totalProductPrice:0.00}€");
            receipt.AppendLine($"   IVA ({Math.Round(product.VatRate * 100, 0)}%): {vatAmount:0.00}€");

            total += totalProductPrice;
        }

        receipt.AppendLine("---------");
        receipt.AppendLine($"Totale: {total:0.00}€");

        return receipt.ToString().TrimEnd();
    }
}
