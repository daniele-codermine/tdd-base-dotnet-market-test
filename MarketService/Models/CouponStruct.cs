using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Models
{
    public class CouponStruct
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public bool IsPercentage { get; set; }
    }
}
