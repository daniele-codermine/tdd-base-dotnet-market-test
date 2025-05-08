using MarketServices;
using FluentAssertions;

namespace MarketServiceTests;

[TestClass]
public class MarketServiceTests
{
	//SUT
	private readonly MarketService _marketService;

	public MarketServiceTests()
	{
		//ARRANGE
		_marketService = new MarketService();
	}

	[TestMethod]
	public void GetProductById_Get_Banana()
	{
		//ACT
		var product = _marketService.GetProductById(1);

		//ASSERT
		product.Should().NotBe(null);
		product!.Id.Should().Be(1);
		product!.Name.Should().Be("Banana");
		product!.Price.Should().Be(2m);
	}

	[DataTestMethod]
	[DataRow(1, "Banana", 2.0)] // Workaround: double qui
	[DataRow(2, "Mela", 3.0)]
	[DataRow(3, "Arancia", 5.0)]
	public void AddProductToCart_Should_Add_Single_Product(int productId, string expectedName, double expectedPriceAsDouble)
	{
		decimal expectedPrice = (decimal)expectedPriceAsDouble; // Workaround: decimal qui


		_marketService.AddProductToCart(productId);
		var cart = _marketService.GetCart();

		// ASSERT
		cart.Should().ContainSingle();
		cart.First().Id.Should().Be(productId);
		cart.First().Name.Should().Be(expectedName);
		cart.First().Price.Should().Be(expectedPrice);
	}

	// Nota: per questo esercizio AddProductToCart di un prodotto non a catalogo genera un'eccezione specifica (ProductNotFoundException)
	[TestMethod]
	public void AddProductToCart_Should_Throw_ProductNotFoundException_When_Product_Not_In_Catalog()
	{
		// ARRANGE
		var productId = 99; // ID di un prodotto non presente nel catalogo

		// ACT
		Action act = () => _marketService.AddProductToCart(productId);

		// ASSERT
		act.Should().Throw<ProductNotFoundException>()
			.WithMessage($"Product with ID {productId} not found in the catalog.");
	}

	[TestMethod]
	public void AddProductToCart_Should_Add_Two_Separate_Items_When_Same_Product_Added_Twice()
	{
		// ARRANGE
		var productId = 2; // Mela

		// ACT
		_marketService.AddProductToCart(productId);
		_marketService.AddProductToCart(productId); 
		var cart = _marketService.GetCart();

		// ASSERT
		cart.Should().HaveCount(2); 
		cart.Should().OnlyContain(p => p.Id == productId);
	}

	// Nota: per questo esercizio si sfrutta AddProductToCart (eventuale raggruppamento prodotti nello scontrino finale)
	[TestMethod]
	public void AddProductToCart_Should_Add_Two_Products_With_Different_Prices()
	{
		// ARRANGE
		var productId1 = 1; // Banana
		var productId2 = 2; // Mela

		// ACT
		_marketService.AddProductToCart(productId1);
		_marketService.AddProductToCart(productId2);
		var cart = _marketService.GetCart();

		// ASSERT
		cart.Should().HaveCount(2);

		cart.Should().Contain(p => p.Id == 1 && p.Name == "Banana" && p.Price == 2m);
		cart.Should().Contain(p => p.Id == 2 && p.Name == "Mela" && p.Price == 3m);
	}

	// Nota: per questo esercizio RemoveProductFromCart di un prodotto non a cart semplicemente ignora
	[TestMethod]
	public void RemoveProductFromCart_Should_Remove_Product_By_Id()
	{
		// ARRANGE
		var productId1 = 1; // Banana
		var productId2 = 2; // Mela

		_marketService.AddProductToCart(productId1);
		_marketService.AddProductToCart(productId2);

		// ACT
		_marketService.RemoveProductFromCart(productId1);
		var cart = _marketService.GetCart();

		// ASSERT
		cart.Should().HaveCount(1);
		cart.Should().NotContain(p => p.Id == productId1);
		cart.Should().Contain(p => p.Id == productId2);
	}

	// Nota: per questo esercizio RemoveProductFromCart di un prodotto non a catalogo genera un'eccezione specifica (ProductNotFoundException)
	[TestMethod]
	public void RemoveProductFromCart_Should_Throw_ProductNotFoundException_When_Product_Not_In_Cart()
	{
		// ARRANGE
		var productId = 99; // ID di un prodotto non presente nel carrello

		// ACT
		Action act = () => _marketService.RemoveProductFromCart(productId);

		// ASSERT
		act.Should().Throw<ProductNotFoundException>()
			.WithMessage($"Product with ID {productId} not found in the cart.");
	}

    // Nota: per questo esercizio AddProductToCart di un prodotto con prezzo decimale (es. 7.6) funziona correttamente la lettura da catalogo ecc.
	[TestMethod]
	public void AddProductToCart_Should_Handle_Product_With_Decimal_Price()
	{
		// ARRANGE
		var productId = 4; // Pera con prezzo 7.6

		// ACT
		_marketService.AddProductToCart(productId);
		var cart = _marketService.GetCart();

		// ASSERT
		cart.Should().ContainSingle();
		cart.First().Id.Should().Be(productId);
		cart.First().Name.Should().Be("Pera");
		cart.First().Price.Should().Be(7.6m); // Verifica gestione decimali
	}

	[TestMethod]
	public void GetCartSummary_Should_Return_Grouped_Product_Summary()
	{
		// ARRANGE
		var productId1 = 1; // Banana
		var productId2 = 2; // Mela
		var productId3 = 4; // Pera

		_marketService.AddProductToCart(productId1); // 2 banane
		_marketService.AddProductToCart(productId1); 
		_marketService.AddProductToCart(productId2); 
		_marketService.AddProductToCart(productId3); 

		// ACT
		var summary = _marketService.GetCartSummary();

		// ASSERT
		summary.Should().HaveCount(3);

		summary.Should().Contain(tuple =>
			tuple.ProductId == productId1 &&
			tuple.Quantity == 2 &&
			tuple.TotalPrice == 4m); // 2 Banane, 4€

		summary.Should().Contain(tuple =>
			tuple.ProductId == productId2 &&
			tuple.Quantity == 1 &&
			tuple.TotalPrice == 3m); // 1 Mela, 3€

		summary.Should().Contain(tuple =>
			tuple.ProductId == productId3 &&
			tuple.Quantity == 1 &&
			tuple.TotalPrice == 7.6m); // 1 Pera 7.6€
	}

	[TestMethod]
	public void GetCartTotal_Should_Return_Correct_Total()
	{
		// ARRANGE
		var productId1 = 1; // Banana
		var productId2 = 2; // Mela
		var productId3 = 4; // Pera

		_marketService.AddProductToCart(productId1); // 2 banane
		_marketService.AddProductToCart(productId1); 
		_marketService.AddProductToCart(productId2); 
		_marketService.AddProductToCart(productId3); 

		// ACT
		var total = _marketService.GetCartTotal();

		// ASSERT
		total.Should().Be(14.6m); // Totale corretto: 4€ + 3€ + 7.6€ = 14.6€
	}

    // Nota: per questo esercizio il coupon è un prezzo fisso
	[TestMethod]
	public void ApplyFixedDiscountCoupon_Should_Subtract_Coupon_From_Total()
	{
		// ARRANGE
		var total = 20m;
		var couponValue = 5m;

		// ACT
		var result = _marketService.ApplyFixedDiscountCoupon(total, couponValue);

		// ASSERT
		result.Should().Be(15m); // Totale atteso: 20 - 5 = 15
	}

    // Nota: per questo esercizio il coupon è un prezzo fisso e si genera InvalidOperationException se il totale diventa negativo
	[TestMethod]
	public void ApplyFixedDiscountCoupon_Should_Throw_Exception_If_Total_Becomes_Negative()
	{
		// ARRANGE
		var total = 10m;
		var couponValue = 15m;

		// ACT
		Action act = () => _marketService.ApplyFixedDiscountCoupon(total, couponValue);

		// ASSERT
		act.Should().Throw<InvalidOperationException>()
			.WithMessage("Total after discount cannot be negative.");
	}

	// Nota: per questo esercizio il coupon è una percentuale e si applica solo se il totale è >= 100
	[TestMethod]
	public void ApplyPercentageDiscount_Should_Apply_10_Percent_If_Total_Is_At_Least_100()
	{
		// ARRANGE
		var total = 100m;
		var discountPercentage = 10m;

		// ACT
		var result = _marketService.ApplyPercentageDiscount(total, discountPercentage);

		// ASSERT
		result.Should().Be(90m); // Totale atteso: 100 - 10% = 90
	}

	[TestMethod]
	public void ApplyPercentageDiscount_Should_Not_Apply_Discount_If_Total_Is_Less_Than_100()
	{
		// ARRANGE
		var total = 99.99m;
		var discountPercentage = 10m;

		// ACT
		var result = _marketService.ApplyPercentageDiscount(total, discountPercentage);

		// ASSERT
		result.Should().Be(99.99m); // Nessuno sconto applicato
	}

     // Nota: per questo esercizio è un 3x2 semplice (dovrebbe ogni 3 prodotti uguali eliminarne uno dal calcolo del toale)
	[TestMethod]
	public void ApplyThreeForTwoDiscount_Should_Apply_Discount_For_Three_Products()
	{
		// ARRANGE
		var productId = 1; // Banana con prezzo 2€
		_marketService.AddProductToCart(productId); // Aggiungiamo 3 banane
		_marketService.AddProductToCart(productId);
		_marketService.AddProductToCart(productId);

		// ACT
		var total = _marketService.ApplyThreeForTwoDiscount();

		// ASSERT
		total.Should().Be(4m); // Totale atteso: 2 prodotti pagati (2€ * 2 = 4€)
	}

    // niente 3x2 ...
	[TestMethod]
	public void ApplyThreeForTwoDiscount_Should_Not_Apply_Discount_If_Less_Than_Three_Products()
	{
		// ARRANGE
		var productId = 1; // Banana 
		_marketService.AddProductToCart(productId); // 2 banane
		_marketService.AddProductToCart(productId);

		// ACT
		var total = _marketService.ApplyThreeForTwoDiscount();

		// ASSERT
		total.Should().Be(4m); 
	}

    // per gruppi da tre ma con altri fuori 3x2...
	[TestMethod]
	public void ApplyThreeForTwoDiscount_Should_Apply_Discount_For_Multiple_Groups_Of_Three()
	{
		// ARRANGE
		var productId = 1; // Banana 
		for (int i = 0; i < 7; i++) // 7 banane
		{
			_marketService.AddProductToCart(productId);
		}

		// ACT
		var total = _marketService.ApplyThreeForTwoDiscount();

		// ASSERT
		total.Should().Be(10m); 
	}

    // Nota: per questo esercizio il bundle è un prezzo fisso e si applica solo se sono presenti i prodotti specifici (Banana, Mela, Arancia)
	[TestMethod]
	public void ApplyBundleDiscount_Should_Apply_Bundle_Price_For_Specific_Products()
	{
		// ARRANGE
		var bananaId = 1; // Banana 
		var appleId = 2;  // Mela 
		var orangeId = 3; // Arancia 

		_marketService.AddProductToCart(bananaId);
		_marketService.AddProductToCart(appleId);
		_marketService.AddProductToCart(orangeId);

		// ACT
		var total = _marketService.ApplyBundleDiscount();

		// ASSERT
		total.Should().Be(8m); // Totale atteso: prezzo bundle "Macedonia" = 8€
	}

}
