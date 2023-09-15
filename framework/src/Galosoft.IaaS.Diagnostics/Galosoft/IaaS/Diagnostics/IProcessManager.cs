using System;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Diagnostics
{
    public interface IProcessManager : IDisposable
    {
        Task StartAsync(IProcessStartContext context);
    }
}
