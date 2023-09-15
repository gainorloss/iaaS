using System.IO;

namespace System.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public static class Tracer
    {
        static Tracer()
        {
            Diagnostics.Trace.AutoFlush = true;
# if !DEBUG
            Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener()
            {
                Name = "Console"
            });

            Diagnostics.Trace.Listeners.Add(new FileTraceListener()
            {
                Name = "File"
            });
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static void Trace(object? value, string category)
        {
            Diagnostics.Trace.WriteLine($"\t{value}", $"“{category}”>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static void Verbose(object? value, string category)
        {
            Trace(value,category);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static void Info(object? value, string category)
        {
            Diagnostics.Trace.TraceInformation($"\t{value}", $"“{category}”>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static void Error(object? value, string category)
        {
            Diagnostics.Trace.TraceError($"\t{value}", $"“{category}”>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceListener"></param>
        public static void Register(TraceListener traceListener)
        {
            Diagnostics.Trace.Listeners.Add(traceListener);
        }
    }

    internal class FileTraceListener : TraceListener
    {
        public FileTraceListener()
        {
            Filter = new FileTraceFilter();
        }

        public FileTraceListener(TraceFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Write(string message)
        {
            TryGetOrCreateTraceFile(out string file);

            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.Write("【{0}】{1}", DateTime.Now.ToLongTimeString(), message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            TryGetOrCreateTraceFile(out string file);

            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.WriteLine("【{0}】{1}", DateTime.Now.ToLongTimeString(), message);
            }
        }

        private static bool TryGetOrCreateTraceFile(out string file)
        {
            var dir = Path.Combine("traces", DateTime.Now.ToString("yyyy/MM"));
            file = Path.Combine(Directory.GetCurrentDirectory(), dir, $"{DateTime.Now.Day}.trace");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return false;
            }

            return true;
        }
    }

    internal class FileTraceFilter : TraceFilter
    {
        public override bool ShouldTrace(TraceEventCache? cache, string source, TraceEventType eventType, int id, string? formatOrMessage, object?[]? args, object? data1, object?[]? data)
        {
            return ((short)eventType) < ((short)TraceEventType.Verbose);
        }
    }
}
