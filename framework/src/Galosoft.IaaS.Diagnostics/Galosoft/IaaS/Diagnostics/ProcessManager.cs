using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Galosoft.IaaS.Diagnostics
{
    public class ProcessManager
        : IProcessManager
    {
        private readonly Process _process;
        private readonly ILogger<ProcessManager> _logger;

        public ProcessManager(ILogger<ProcessManager> logger)
        {
            _process = new Process();
            _logger = logger;
        }

        public async Task StartAsync(IProcessStartContext ctx)
        {
            await Task.Run(() =>
            {
                var arguments = string.Join(";", ctx.Arguments);

                var startInfo = _process.StartInfo;
                startInfo.FileName = ctx.FileName;
                startInfo.WorkingDirectory = ctx.WorkingDirectory;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                _process.Start();

                _process.OutputDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                        return;
                    _logger.LogInformation(e.Data);
                };
                if (ctx.OnReceiveData != null)
                    _process.OutputDataReceived += (sender, e) => ctx.OnReceiveData(0, e.Data);
                _process.BeginOutputReadLine();

                _process.ErrorDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                        return;
                    _logger.LogError(e.Data);
                };
                if (ctx.OnReceiveData != null)
                    _process.OutputDataReceived += (sender, e) => ctx.OnReceiveData(1, e.Data);
                _process.BeginErrorReadLine();

                _process.WaitForExit();
            });
        }

        public void Dispose()
        {
            _process.Close();
            _process.Dispose();
        }
    }
}
