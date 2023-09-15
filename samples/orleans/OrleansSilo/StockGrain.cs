using Orleans;
using System.Diagnostics;

namespace OrleansSilo
{
    public class StockGrain
        : Grain, IStockGrain
    {
        private int QTY = 0;
        private readonly ILogger<StockGrain> _logger;

        public StockGrain(ILogger<StockGrain> logger)
        {
            _logger = logger;
        }

        public async Task ChangeQtyAsync()
        {
            await Task.Delay(10);
            QTY++;
            _logger.LogWarning("Qty:{0}", QTY);
        }
    }

    public interface IStockGrain : IGrainWithGuidKey
    {
        Task ChangeQtyAsync();
    }
}
