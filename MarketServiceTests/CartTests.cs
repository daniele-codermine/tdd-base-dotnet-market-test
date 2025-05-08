using FluentAssertions;
using MarketServices;
using MarketServices.Models;

namespace MarketServiceTests;

[TestClass]
public class CartTests
{
    private readonly MarketService _marketService;

    public CartTests()
    {
        _marketService = new MarketService();
    }

    [TestMethod]
    public void AddProduct_Should_Add_Single_Product_With_Fixed_Price()
    {
        var cart = new Cart();
        var product = _marketService.GetProductById(1); // Banana, 2€

        cart.AddProduct(product, 1);

        cart.Total.Should().Be(2m);
    }

    [TestMethod]
    public void AddProduct_Should_Add_Multiple_Products_With_Different_Prices()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€
        var apple = _marketService.GetProductById(2); // Mela, 3€

        cart.AddProduct(banana, 1); // 2€
        cart.AddProduct(apple, 1); // 3€

        cart.Total.Should().Be(5m); // 5€
    }

    [TestMethod]
    public void RemoveProduct_Should_Remove_Product_From_Cart()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€
        var apple = _marketService.GetProductById(2); // Mela, 3€

        cart.AddProduct(banana, 1);
        cart.AddProduct(apple, 1);

        cart.RemoveProduct(banana);

        cart.Total.Should().Be(3m); // Solo la Mela
    }

    [TestMethod]
    public void UpdateProductQuantity_Should_Update_Quantity_And_Total()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€

        cart.AddProduct(banana, 1); // 2€

        cart.UpdateProductQuantity(banana, 3); // 3 x 2€

        cart.Total.Should().Be(6m); // 3 x 2€ = 6€
    }

    [TestMethod]
    public void AddProduct_Should_Handle_Decimal_Prices_Correctly()
    {
        var cart = new Cart();
        var pear = _marketService.GetProductById(4); // Pera, 3.99€

        cart.AddProduct(pear, 2); // 7.98€

        cart.Total.Should().Be(7.98m); // 7.98€
    }

    [TestMethod]
    public void ApplyFixedDiscount_Should_Subtract_Discount_From_Total()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€
        var apple = _marketService.GetProductById(2); // Mela, 3€

        cart.AddProduct(banana, 1);
        cart.AddProduct(apple, 1);

        cart.ApplyFixedDiscount(5); // Sconto fisso di 5€

        cart.Total.Should().Be(0m); // 5€ di sconto su 5€ di totale
    }

    [TestMethod]
    public void ApplyPercentageDiscount_Should_Subtract_Percentage_From_Total()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€
        var apple = _marketService.GetProductById(2); // Mela, 3€

        cart.AddProduct(banana, 50); // 50 x 2€ = 100€
        cart.AddProduct(apple, 50); // 50 x 3€ = 150€

        cart.ApplyPercentageDiscount(10); // Sconto 10% su 250€

        cart.Total.Should().Be(225m); // 250€ - 10% = 225€
    }

    [TestMethod]
    public void Apply3x2Discount_Should_Give_Free_Product_When_3_Products_Are_Added()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€

        cart.AddProduct(banana, 3);

        cart.Apply3x2Discount(); // La terza banana è gratuita

        cart.Total.Should().Be(4m); // 2 banane pagate, totale 4€
    }

    [TestMethod]
    public void ApplyBundleDiscount_Should_Apply_Bundle_Price_When_Products_Are_Added()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€
        var apple = _marketService.GetProductById(2); // Mela, 3€
        var orange = _marketService.GetProductById(3); // Arancia, 5€

        cart.AddProduct(banana, 1);
        cart.AddProduct(apple, 1);
        cart.AddProduct(orange, 1);

        cart.ApplyBundleDiscount("Macedonia", 8m); // Prezzo per il bundle: 8€

        cart.Total.Should().Be(8m);
    }

    [TestMethod]
    public void TotalWithVAT_Should_Handle_Vat_And_Exempt_Correctly()
    {
        var cart = new Cart();
        var banana = _marketService.GetProductById(1); // Banana, 2€ con IVA 10%
        var orange = _marketService.GetProductById(3); // Arancia, 5€ con IVA 20%
        var pineapple = _marketService.GetProductById(5); // Ananas, 8.50€ con IVA esente

        cart.AddProduct(banana, 1); // Prezzo: 2€ + IVA
        cart.AddProduct(orange, 1); // Prezzo: 5€ + IVA
        cart.AddProduct(pineapple, 3); // Prezzo: 25.50€

        var totalWithVAT = cart.TotalWithVAT; // Totale con IVA

        totalWithVAT.Should().Be(33.70m); // 2€ + (10% di IVA) = 2.20€, 5€ + (20% di IVA) = 6€, 3 * 8.50€ = 25.50€, Totale = 33.70€
    }
    [TestMethod]
    public void GenerateReceipt_Should_Return_Correct_Receipt_Format()
    {
        var cart = new Cart();

        var banana = _marketService.GetProductById(1); // 2 x 2€
        var apple = _marketService.GetProductById(2);   // 1 x 3€
        var orange = _marketService.GetProductById(3); // 3 x 5€
        var pear = _marketService.GetProductById(4);   // 1 x 3.99€
        var pineapple = _marketService.GetProductById(5); // 4 x 8.50€

        cart.AddProduct(banana, 2);
        cart.AddProduct(apple, 1);
        cart.AddProduct(orange, 3);
        cart.AddProduct(pear, 1);
        cart.AddProduct(pineapple, 4);

        var expectedReceipt =
    @"Scontrino
---------
Banana - 
  2 x 2€ = 4.40€
   IVA (10%): 0.40€
Mela - 
  1 x 3€ = 3.30€
   IVA (10%): 0.30€
Arancia - 
  3 x 5€ = 18.00€
   IVA (20%): 3.00€
Pera - 
  1 x 3.99€ = 4.15€
   IVA (4%): 0.16€
Ananas - 
  4 x 8.50€ = 34.00€
   IVA (0%): 0.00€
---------
Totale: 63.85€";

        var actualReceipt = cart.GenerateReceipt();

        actualReceipt.Should().Be(expectedReceipt);
    }
}
