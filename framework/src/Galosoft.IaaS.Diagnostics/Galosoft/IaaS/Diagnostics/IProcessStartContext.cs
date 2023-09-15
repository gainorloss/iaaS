using System;
using System.Collections.Generic;

namespace Galosoft.IaaS.Diagnostics
{
    public interface IProcessStartContext
    {
        string FileName { get; }
        string WorkingDirectory { get; }
        IReadOnlyList<string> Arguments { get; }
        Action<short, string> OnReceiveData { get; }
    }
}
