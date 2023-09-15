using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace OrleansSilo.Controllers
{
    [Route("api/stocks/[action]")]
    public class StocksController
        : Controller
    {
        private readonly IGrainFactory _factory;

        public StocksController(IGrainFactory factory)
        {
            _factory = factory;
        }
        public async Task ChangeAsync()
        {
            var guid = Guid.Empty;
            var basic = _factory.GetGrain<IStockGrain>(guid);
            await basic.ChangeQtyAsync();
        }
    }
}
