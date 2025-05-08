using MarketServices;
using FluentAssertions;
using MS = MarketServices.MarketService;

namespace MarketServiceTests;

[TestClass]
public class MarketServiceTests
{
	//SUT
	private readonly MS _marketService;

	public MarketServiceTests()
	{
		//ARRANGE
		_marketService = new MS();
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

}
