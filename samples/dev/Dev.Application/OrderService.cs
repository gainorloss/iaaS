using Dapper;
using Dev.ConsoleApp.Entities;
using Dev.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.Application
{
    public class OrderService
    {
        private readonly OrderDbContext _ctx;

        public OrderService(OrderDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Order>> ListAsync()
        {
            //var cnn = _ctx.Database.GetDbConnection();
            //var os = await cnn.QueryAsync("select top 10 id from t_orders");
            var orders = await _ctx.Query<Order>().Take(100).ToListAsync();
            return orders;
        }
    }
}
