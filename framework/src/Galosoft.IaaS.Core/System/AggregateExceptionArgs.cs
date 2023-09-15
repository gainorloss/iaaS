using System.Linq;
using System.Text;

namespace System
{
    public class AggregateExceptionArgs
        : EventArgs
    {
        public AggregateExceptionArgs(params Exception[] innerExceptions)
        {
            innerExceptions = innerExceptions.Where(innerException => innerException != null)
                .ToArray();

            if (innerExceptions != null && innerExceptions.Any())
                AggregateException = new AggregateException(innerExceptions);
        }

        public AggregateException AggregateException { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var innerException in AggregateException.InnerExceptions)
                sb.AppendFormat(
                    "“异常类型”：{1}{0}“异常来源”：{2}{0}“异常信息”：{3}{0}“异常堆栈”：{4}{0}",
                    Environment.NewLine,
                    innerException.GetType(),
                    innerException.Source,
                    innerException.Message,
                    innerException.StackTrace);
            return sb.ToString();
        }
    }
}
