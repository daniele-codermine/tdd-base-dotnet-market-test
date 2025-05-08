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
	[DataRow(1, "Banana", 2.0)]
	[DataRow(2, "Mela", 3.0)]
	public void GetProductById_Get_Product(int id, string name, double price)
	{
		//ACT
		var product = _marketService.GetProductById(id);

		//ASSERT
		product.Should().NotBe(null);
		product!.Id.Should().Be(id);
		product!.Name.Should().Be(name);
		product!.Price.Should().Be((decimal)price);
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(1, 2.0)]
	[DataRow(2, 3.0)]
	[DataRow(3, 5.0)]
	public void AddProductToCheck_Add_Product(int id, double price)
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var product = _marketService.GetProductById(id);
		productsBought.Add(new CheckProduct(product));

		//ACT
		var check = _marketService.CreateCheck(productsBought);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.Products.Should().ContainSingle(p => p.Id == id);
		check!.FinalPrice.Should().Be((decimal)price);

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void RemoveProductFromCheck_Remove_Product()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = _marketService.GetProductById(1);
		var mela = _marketService.GetProductById(2);
		productsBought.Add(new CheckProduct(banana));
		productsBought.Add(new CheckProduct(mela));

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		check = _marketService.RemoveProductFromCheck(check.Id, banana.Id);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.Products.Should().NotContain(p => p.Id == banana.Id);
		check!.FinalPrice.Should().Be(3m);

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void RemoveFakeProductFromCheck_Throw_Exception()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = _marketService.GetProductById(1);
		var mela = _marketService.GetProductById(2);
		productsBought.Add(new CheckProduct(banana));
		productsBought.Add(new CheckProduct(mela));

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		var act = () => _marketService.RemoveProductFromCheck(check.Id, 999);

		//ASSERT
		act.Should().Throw<Exception>().WithMessage("Product with ID 999 not found in the check.");

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void RemoveProductFromFakeCheck_Throw_Exception()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = _marketService.GetProductById(1);
		var mela = _marketService.GetProductById(2);
		productsBought.Add(new CheckProduct(banana));
		productsBought.Add(new CheckProduct(mela));

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		var act = () => _marketService.RemoveProductFromCheck(Guid.Empty, banana.Id);

		//ASSERT
		act.Should().Throw<Exception>().WithMessage("Check with ID 00000000-0000-0000-0000-000000000000 not found.");

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void UpdateProductInCheck_Update_Product()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		mela.Quantity = 5;
		check = _marketService.UpdateProductInCheck(check.Id, mela);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.Products.Should().Contain(p => p.Id == mela.Id);
		check!.Products.First(p => p.Id == mela.Id).Quantity.Should().Be(mela.Quantity);

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void UpdateFakeProductInCheck_Throw_Exception()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		var fakeProduct = new CheckProduct();
		fakeProduct.Id = 999;
		var act = () => _marketService.UpdateProductInCheck(check.Id, fakeProduct);

		//ASSERT
		act.Should().Throw<Exception>().WithMessage("Product with ID 999 not found in the check.");

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void UpdateProductInFakeCheck_Throw_Exception()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		mela.Quantity = 5;
		var act = () => _marketService.UpdateProductInCheck(Guid.Empty, mela);

		//ASSERT
		act.Should().Throw<Exception>().WithMessage("Check with ID 00000000-0000-0000-0000-000000000000 not found.");

		_marketService.DeleteCheck(check.Id);
	}

	[TestMethod]
	[DoNotParallelize]
	public void AddDecimalPricesProducts_Correct_Total()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var pera = new CheckProduct(_marketService.GetProductById(4));
		var kiwi = new CheckProduct(_marketService.GetProductById(6));
		productsBought.Add(pera);
		productsBought.Add(kiwi);

		//ACT
		var check = _marketService.CreateCheck(productsBought);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(6.5m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyDiscountFixed_Correct_Total()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		var discount = new FixedDiscount();
		discount.Amount = 2;

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(3m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyDiscountPercentage_Correct_Total()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		banana.SoldPrice = 50;
		var mela = new CheckProduct(_marketService.GetProductById(2));
		mela.SoldPrice = 50;
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		var discount = new PercentageDiscount();
		discount.Percentage = 10;
		discount.Threshold = 100;

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(90m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyMultiBuyDiscount_Correct_Total()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		banana.Quantity = 3;
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		var discount = new MultiBuyDiscount();
		discount.MultiBuy = (banana.Id, 3, 1);

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(7m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyMultiBuyDiscountMultiple_Correct_Total()
	{
		//ARRANGE
		var productsBought = new List<CheckProduct>();
		var banana = new CheckProduct(_marketService.GetProductById(1));
		banana.Quantity = 7;
		var mela = new CheckProduct(_marketService.GetProductById(2));
		productsBought.Add(banana);
		productsBought.Add(mela);

		var check = _marketService.CreateCheck(productsBought);

		var discount = new MultiBuyDiscount();
		discount.MultiBuy = (banana.Id, 3, 1);

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(13m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyDiscountBundle_Correct_Total()
	{
		//ARRANGE
		var banana = new CheckProduct(_marketService.GetProductById(1));
		var mela = new CheckProduct(_marketService.GetProductById(2));
		var arancia = new CheckProduct(_marketService.GetProductById(3));
		var pera = new CheckProduct(_marketService.GetProductById(4));
		var uva = new CheckProduct(_marketService.GetProductById(5));
		var kiwi = new CheckProduct(_marketService.GetProductById(6));
		var productsBought = new List<CheckProduct>{ banana, mela, arancia, pera, uva, kiwi };

		var discount = new BundleDiscount();
		discount.Bundle = ([banana.Id, mela.Id, arancia.Id], 8);

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(16.3m);
	}

	[TestMethod]
	[DoNotParallelize]
	public void ApplyDiscountBundleMultiProducts_Correct_Total()
	{
		//ARRANGE
		var banana = new CheckProduct(_marketService.GetProductById(1));
		banana.Quantity = 2;
		var mela = new CheckProduct(_marketService.GetProductById(2));
		mela.Quantity = 3;
		var arancia = new CheckProduct(_marketService.GetProductById(3));
		arancia.Quantity = 2;
		var pera = new CheckProduct(_marketService.GetProductById(4));
		var uva = new CheckProduct(_marketService.GetProductById(5));
		var kiwi = new CheckProduct(_marketService.GetProductById(6));
		var productsBought = new List<CheckProduct>{ banana, mela, arancia, pera, uva, kiwi };

		var discount = new BundleDiscount();
		discount.Bundle = ([banana.Id, mela.Id, arancia.Id], 8);

		var check = _marketService.CreateCheck(productsBought);

		//ACT
		check = _marketService.ApplyDiscount(check.Id, discount);

		//ASSERT
		check.Should().NotBe(null);
		check!.Products.Should().NotBeEmpty();
		check!.FinalPrice.Should().Be(27.3m);
	}
}