using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketService.Utils;

namespace MarketService.Models
{
    public class BoundleItem
    {
        public string Name { get; set; }

        List<Product> boundleProduct = [];
        public decimal FinalPrice { get; set; }
        public decimal Discount => boundleProduct.Sum(x => x.Price) - FinalPrice;

        public BoundleItem(string name, List<Product> bundle)
        {
            this.Name = name;
            this.boundleProduct = bundle;
        }

        public void AddItem(Product p)
        {
            boundleProduct.Add(p);
        }

        public bool IsValid(List<Product> boundleProduct)
        {
            List<int> itemsFounds = [];
            foreach (var item in  boundleProduct)
            {
                if (this.boundleProduct.Contains(item))
                {
                    itemsFounds.AddUnique(item.Id);
                }
            }
            return itemsFounds.Count == boundleProduct.Count;
        }

        public Dictionary<int, decimal> GetProductPrice()
        {
            Dictionary<int, decimal> result = [];
            foreach (var item in boundleProduct)
            {
                result.Add(item.Id, item.Price);
            }
            return result;
        }
    }
}
