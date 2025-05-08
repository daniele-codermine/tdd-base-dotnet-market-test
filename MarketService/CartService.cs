using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketService.Models;
using MarketService.Utils;

namespace MarketServiceCart
{
    public class CartService
    {
        public static string DISCOUNT3X2 = "DISCOUNT3X2";

        private List<CartItem> cart = [];
        private Dictionary<string, CouponStruct> Coupons = [];
        private List<BoundleItem> boundleItems = [];
        private Dictionary<int, decimal> IVA = [];

        private bool bundleMode = false;

        /// <summary>
        /// Aggiunge un prodotto al carrello
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        public void AddProductToCart(Product product, int quantity)
        {
            cart ??= [];

            var newItem = new CartItem(product, quantity);
            newItem.Id = cart.Count + 1;
            cart.AddUnique(newItem);
        }

        /// <summary>
        /// Ritorna il carrello
        /// </summary>
        /// <returns></returns>
        public List<CartItem> GetCart()
        {
            return cart;
        }

        /// <summary>
        /// Ritorna il totale del carrello senza IVA e con gli sconti applicati.
        /// NOTA: la modalità boundle inibisce gli sconti
        /// </summary>
        /// <returns></returns>
        public decimal GetTaxableBase()
        {
            var parzial = cart.Sum(x => x.TotalPrice);

            ApplySpecialCoupons(ref parzial);

            ApplyCoupons(ref parzial);

            ApplyBoundleMode(ref parzial);

            return parzial;
        }

        /// <summary>
        /// Applica la modalita boundle
        /// </summary>
        /// <param name="parzial"></param>
        private void ApplyBoundleMode(ref decimal parzial)
        {
            if (!bundleMode)
            {
                return;
            }

            List<int> productAlreadyDiscounted = [];
            foreach (var boundleItem in boundleItems)
            {
                var getProductPrice = boundleItem.GetProductPrice();
                if (boundleItem.IsValid(cart.Select(x => x.Product).ToList()))
                {
                    foreach (var pp in getProductPrice)
                    {
                        if (productAlreadyDiscounted.Contains(pp.Key))
                            continue;

                        parzial -= pp.Value;
                        productAlreadyDiscounted.Add(pp.Key);
                    }

                    parzial += boundleItem.FinalPrice;
                }
            }
        }

        /// <summary>
        /// Applica gli sconti percentuali e statici al totale
        /// </summary>
        /// <param name="parzial"></param>
        private void ApplyCoupons(ref decimal parzial)
        {
            if (bundleMode)
            {
                return;
            }

            foreach (var coupon in Coupons)
            {
                if (skip_coupon())
                    continue;

                if (coupon.Value.IsPercentage)
                    parzial -= (parzial * coupon.Value.Discount / 100);
                else
                    parzial -= coupon.Value.Discount;

                bool skip_coupon()
                {
                    if (coupon.Value.Code == DISCOUNT3X2)
                        return true;
                    return false;
                }
            }
        }

        /// <summary>
        /// Applica glis conti speciali al totale
        /// </summary>
        /// <param name="total"></param>
        private void ApplySpecialCoupons(ref decimal total)
        {
            if (bundleMode)
            {
                return;
            }

            foreach (var coupon in Coupons)
            {
                if (coupon.Value.Code == DISCOUNT3X2)
                {
                    var totalItems = cart.Sum(x => x.Quantity);
                    var timesToIterate = totalItems / 3;
                    if (timesToIterate > 0)
                    {
                        var orderList = cart.OrderByDescending(x => x.Product.Price).ToList();
                        List<int> fullItemList = GetExtendendCart(orderList);
                        List<Product> itemsToDiscount = [];
                        for (int i = 0; i < timesToIterate; i++)
                        {
                            var indexToUse = fullItemList.Count - 1 - i;
                            itemsToDiscount.Add(orderList.FirstOrDefault(x=>x.Id == fullItemList[indexToUse]).Product);
                        }
                        foreach (var itemToDiscount in itemsToDiscount)
                        {
                            total -= itemToDiscount.Price;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ritornal a lista degli oggetti del carrello espansa in base alla quantita'
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        List<int> GetExtendendCart(List<CartItem> cart)
        {
            List<int> fullItemList = [];
            foreach (var item in cart)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    fullItemList.Add(item.Product.Id);
                }
            }
            return fullItemList;
        }

        /// <summary>
        /// Rimuove un prodotto dal carrello
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveProductFromCart(int ID)
        {
            if (cart == null)
            {
                return;
            }

            if (cart.Count == 0)
            {
                return;
            }

            cart.RemoveAll(item => item.Id == ID);
        }

        /// <summary>
        /// Ritorna i prodotti nel carrello in base all'ID del prodotto
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<CartItem> GetProductsInCartByProductID(int productID)
        {
            return cart.Where(x => x.Product.Id == productID).ToList();
        }

        /// <summary>
        /// Aggiorna un prodotto nel carrello
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateProductInCart(CartItem item)
        {
            if (cart == null)
            {
                return false;
            }
            if (cart.Count == 0)
            {
                return false;
            }
            var index = cart.FindIndex(x => x.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            cart[index] = item;
            return true;
        }

        /// <summary>
        /// Aggiunge un coupon alla lista coupons
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="isPercentage"></param>
        public void AddCoupon(string code, decimal value, bool isPercentage = false)
        {
            Coupons ??= [];

            CouponStruct coupon = new CouponStruct
            {
                Code = code,
                Discount = value,
                IsPercentage = isPercentage
            };

            if (Coupons.Count == 0)
            {
                Coupons.Add(code, coupon);
                return;
            }

            Coupons.TryAdd(code, coupon);
        }

        /// <summary>
        /// Attiva/disattiva la modalita' bundle
        /// </summary>
        public void EnableBundleMode()
        {
            bundleMode = !bundleMode;
        }

        /// <summary>
        /// Aggiunge un bundle al carrello
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="finalValue"></param>
        public void AddBundle(string name, List<Product> items, decimal finalValue)
        {
            BoundleItem boundleItem = new BoundleItem(name, items);
            boundleItem.FinalPrice = finalValue;
            boundleItems.Add(boundleItem);

        }

        /// <summary>
        /// Ritorna il totale del carrello con IVA
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            var partial = GetTaxableBase();
            var ivas = decimal.Zero;
            foreach (var iva in IVA)
            {
                var item = cart.FirstOrDefault(x => x.Product.Id == iva.Key);
                if (item != null)
                {
                    ivas +=  (item.TotalPrice * iva.Value / 100);
                }
            }

            return partial + ivas;
        }

        /// <summary>
        /// Aggiunge l'iva per prodotto
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="IVA"></param>
        public void AddIVA(int productID, decimal IVA)
        {
            if (this.IVA == null)
            {
                this.IVA = [];
            }
            if (this.IVA.Count == 0)
            {
                this.IVA.Add(productID, IVA);
                return;
            }
            this.IVA.TryAdd(productID, IVA);
        }

        /// <summary>
        /// Stampa la fattura
        /// </summary>
        /// <returns></returns>
        public string PrintInvoice()
        {
            var stringToPrint = "";
            foreach (var item in cart)
            {
                stringToPrint += $"{item.Product.Name} - {item.Quantity} - {item.TotalPrice}\n";
            }

            stringToPrint += $"Total: {GetTotal()}\n";

            return stringToPrint;
        }
    }
}
