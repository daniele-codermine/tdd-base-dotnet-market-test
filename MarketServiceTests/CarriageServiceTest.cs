using MarketServices;
using FluentAssertions;

namespace MarketServicesTest;

[TestClass]
public class CarriageServiceTest
{
	//SUT
	private readonly CarriageService _sut;

	public CarriageServiceTest()
	{
		//ARRANGE
		_sut = new CarriageService(new MarketService());
	}

	[TestMethod]
	public void AddItem_Add()
	{
		//ACT
		var carriageItem = _sut.AddItem(1, 1);

		//ASSERT
		carriageItem.Should().NotBe(null);
		carriageItem.IdProduct.Should().Be(1);
		carriageItem.Price.Should().Be(2);
	}

	[TestMethod]
    [DataRow(1,1,2)]
    [DataRow(2,1,3)]
    [DataRow(3,1,5)]
    [DataRow(1,3,6)]
    [DataRow(2,4,12)]
    [DataRow(3,5,25)]
    [DataRow(4,1,5.5)]
    [DataRow(4,2,11)]
	public void AddItems_Add(int idProduct, int qta, double priceExpected)
	{
		//ACT
		var item = _sut.AddItem(idProduct, qta);

		//ASSERT
		item.Should().NotBe(null);
		item.IdProduct.Should().Be(idProduct);
		item.Price.Should().Be((decimal)priceExpected);
	}

	[TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
	public void GetItemById_Get(int idProduct)
	{
		//ACT
		var item = _sut.AddItem(idProduct, 1);
		var result = _sut.GetItemById(item.Id);

		//ASSERT
		result.Should().NotBe(null);
		result.Id.Should().Be(item.Id);
		result.IdProduct.Should().Be(item.IdProduct);
		result.Price.Should().Be(item.Price);
	}

	[TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
	public void RemoveItem_Remove(int idProduct)
	{
		//ACT
		var item = _sut.AddItem(idProduct, 1);
		var result = _sut.RemoveItemById(item.Id);
		var result2 = _sut.GetItemById(item.Id);

		//ASSERT
		result.Should().Be(true);
		result2.Should().BeNull();
	}


	[TestMethod]
    [DataRow(1,5,10)]
    [DataRow(2,7,21)]
    [DataRow(3,2,10)]
    [DataRow(4,10,55)]
	public void UpdateQtaItem_Update(int idProduct, int qta, double priceExpected)
	{
		//ACT
		var item = _sut.AddItem(idProduct, 1);
		var itemUpdated = _sut.UpdateQtaItem(item.Id, qta);

		//ASSERT
		itemUpdated.Qta.Should().Be(qta);
		itemUpdated.Price.Should().Be((decimal)priceExpected);
	}


	[TestMethod]
	[DataRow(155)]
	public void GetTotal_Get(double expectedResult)
	{
		//ACT
		_sut.AddItem(1, 10);
		_sut.AddItem(2, 10);
		_sut.AddItem(3, 10);
		_sut.AddItem(4, 10);
		var result = _sut.GetTotal();

		//ASSERT
		result.Should().Be((decimal)expectedResult);
	}


	[TestMethod]
	[DataRow(1,154)]
	[DataRow(10,145)]
	[DataRow(5,150)]
	public void GetTotalWithFixedDiscount_Get(double discount, double expectedResult)
	{
		//ACT
		_sut.AddItem(1, 10);
		_sut.AddItem(2, 10);
		_sut.AddItem(3, 10);
		_sut.AddItem(4, 10);
		var result = _sut.GetTotalWithDiscount((decimal)discount);

		//ASSERT
		result.Should().Be((decimal)expectedResult);
	}


	[TestMethod]
	[DataRow(100,10,139.5)]
	public void GetTotalWithPercDiscount_Get(double limit, double discountPerc, double expectedResult)
	{
		//ACT
		_sut.AddItem(1, 10);
		_sut.AddItem(2, 10);
		_sut.AddItem(3, 10);
		_sut.AddItem(4, 10);
		var discount = _sut.GetDiscount((decimal)limit, (decimal)discountPerc);
		var result = _sut.GetTotalWithDiscount((decimal)discount);

		//ASSERT
		result.Should().Be((decimal)expectedResult);
	}
}
