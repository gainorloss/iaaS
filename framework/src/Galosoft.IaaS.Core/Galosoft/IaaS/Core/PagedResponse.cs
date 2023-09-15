using System.Collections;
using System.Collections.Generic;

namespace Galosoft.IaaS.Core
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> items, int total)
        {
            Items = items;
            Total = total;
        }

        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; }
    }
}