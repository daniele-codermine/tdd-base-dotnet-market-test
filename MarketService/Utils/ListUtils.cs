using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Utils
{
    public static class ListUtils
    {
        public static void AddUnique<T>(this ICollection<T> list, T item)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
