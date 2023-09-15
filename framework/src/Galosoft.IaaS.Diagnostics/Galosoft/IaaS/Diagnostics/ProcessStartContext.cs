using System;
using System.Collections.Generic;

namespace Galosoft.IaaS.Diagnostics
{
    public class ProcessStartContext
        : IProcessStartContext
    {
        private List<string> _arguments;
        public ProcessStartContext(
            string fileName,
            string workingDirectory)
        {
            FileName = fileName;
            WorkingDirectory = workingDirectory;
            _arguments = new List<string>();
        }

        public ProcessStartContext AddCommands(params string[] cmds)
        {
            _arguments.AddRange(cmds);
            return this;
        }

        public ProcessStartContext ReceiveData(Action<short, string> onReceiveData)
        {
            OnReceiveData = onReceiveData;
            return this;
        }

        public string FileName { get; protected set; }
        public string WorkingDirectory { get; protected set; }
        public IReadOnlyList<string> Arguments => _arguments.AsReadOnly();
        public Action<short, string> OnReceiveData { get; protected set; }
    }
}