using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using MarketService.Models;

using MarketServiceCart;

namespace MarketServiceTests
{
    [TestClass]
    public class CartServiceTests
    {
        private readonly CartService cartService;

        public CartServiceTests()
        {
            //ARRANGE
            cartService = new CartService();
        }

        [TestMethod]
        public void AddProductToCart_Add_Banana()
        {
            Product product = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };

            //ACT
            cartService.AddProductToCart(product, 1);
            //ASSERT
            var cart = cartService.GetCart();
            cart[0].Product.Id.Should().Be(1);
            cart[0].Product.Name.Should().Be("Banana");
            cart[0].Product.Price.Should().Be(2m);
            cart[0].Quantity.Should().Be(1);
            cart[0].TotalPrice.Should().Be(2);
            cart.Count.Should().Be(1);

        }

        [TestMethod]
        public void AddTwoProductsToCart_Add_Banana_And_Apple()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddProductToCart(apple, 1);
            //ASSERT
            var cart = cartService.GetCart();
            var totalPrice = cartService.GetTaxableBase();

            cart.Count.Should().Be(2);
            totalPrice.Should().Be(5);
        }

        [TestMethod]
        public void RemoveProduct()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddProductToCart(apple, 1);
            cartService.RemoveProductFromCart(1);
            //ASSERT
            var cart = cartService.GetCart();
            var totalPrice = cartService.GetTaxableBase();
            cart.Count.Should().Be(1);
            totalPrice.Should().Be(3);//remove the banana so apple is the only one left

        }

        [TestMethod]
        public void UpdateProduct()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            var bananas = cartService.GetProductsInCartByProductID(banana.Id);
            var firstBanana = bananas.First();
            firstBanana.Quantity = 2;
            cartService.UpdateProductInCart(firstBanana);
            //ASSERT
            var cart = cartService.GetCart();
            var totalPrice = cartService.GetTaxableBase();
            cart.Count.Should().Be(1);
            totalPrice.Should().Be(4);//2 bananas at 2 each
        }

        [TestMethod]
        public void UpdateProductNotInCart()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };

            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            CartItem FAKECartItem = new CartItem(apple, 1);
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.UpdateProductInCart(FAKECartItem);
            //ASSERT
            var cart = cartService.GetCart();
            var totalPrice = cartService.GetTaxableBase();
            cart.Count.Should().Be(1);
            totalPrice.Should().Be(2);//1 banana at 2 each
        }

        [TestMethod]
        public void CheckDecimalValue()
        {
            Product bananaDecimal = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2.5m
            };
            //ACT
            cartService.AddProductToCart(bananaDecimal, 1);
            //Assert
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(2.5m);
        }

        [TestMethod]
        public void ApplyPercentageDiscountCoupon()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };

            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddCoupon("DISCOUNT10", 10, true);

            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(1.8m); // 2 - 10% = 1.8
        }

        [TestMethod]
        public void ApplyStaticDiscountCoupon()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };

            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddCoupon("DISCOUNT1", 1);

            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(1); // 2 - 1 = 1
        }

        [TestMethod]
        public void Apply3x2DiscountCoupon()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            //ACT
            cartService.AddProductToCart(banana, 3);
            cartService.AddCoupon(CartService.DISCOUNT3X2, 0);
            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(4); // 3 bananas at 2 each - 1 banana free = 4
        }

        [TestMethod]
        public void Apply3x2DiscountOnMultipleCoupon()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "apple",
                Price = 1m
            };
            //ACT
            cartService.AddProductToCart(banana, 3);
            cartService.AddProductToCart(apple, 3);
            cartService.AddCoupon(CartService.DISCOUNT3X2, 0);
            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(7); // 3 bananas at 2 each and 3 apple at 1 = 9  - 2 apple free = 7
        }

        [TestMethod]
        public void EnableBundleMode()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddProductToCart(apple, 1);
            cartService.EnableBundleMode();
            cartService.AddBundle("applebanana", [banana, apple], 1);
            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(1); // 2 + 3 - 4 = 1
        }

        [TestMethod]
        public void EnableBundleModeMultipleMixed()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            Product Chicken = new Product
            {
                Id = 3,
                Name = "Chicken",
                Price = 1m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddProductToCart(apple, 1);
            cartService.AddProductToCart(Chicken, 1);
            cartService.EnableBundleMode();
            cartService.AddBundle("applebanana", [banana, apple], 1);
            cartService.AddBundle("chickenBanana", [Chicken, banana], 1);
            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(2);
        }

        [TestMethod]
        public void BoundleNotFound()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            Product apple = new Product
            {
                Id = 2,
                Name = "Apple",
                Price = 3m
            };
            Product Chicken = new Product
            {
                Id = 3,
                Name = "Chicken",
                Price = 1m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddProductToCart(apple, 1);
            cartService.EnableBundleMode();
            cartService.AddBundle("chickenBanana", [Chicken, banana], 1);
            //ASSERT
            var totalPrice = cartService.GetTaxableBase();
            totalPrice.Should().Be(5);
        }


        [TestMethod]
        public void ApplyIVA()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            cartService.AddIVA(1, 21);
            //ASSERT
            var totalPrice = cartService.GetTotal();
            totalPrice.Should().Be(2.42m); // 2 + 21% = 2.42
        }

        [TestMethod]
        public void PrintInvoice()
        {
            Product banana = new Product
            {
                Id = 1,
                Name = "Banana",
                Price = 2m
            };
            //ACT
            cartService.AddProductToCart(banana, 1);
            var print = cartService.PrintInvoice();
            //ASSERT
            var totalPrice = cartService.GetTotal();
            totalPrice.Should().Be(2); // 2 + 0% = 2
            print.Should().Contain("Banana - 1 - 2\n");
            print.Should().Contain("Total: 2");
        }
    }
}
