using System.Collections;
using System.Collections.Generic;

namespace Galosoft.IaaS.Core
{
    public class PagedResultDto<T>
    {
        public PagedResultDto(IEnumerable<T> items, int total)
        {
            Items = items;
            Total = total;
        }

        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; }
    }
}