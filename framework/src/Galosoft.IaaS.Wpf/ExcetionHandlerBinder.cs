using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Galosoft.IaaS.Wpf
{
    internal static class ExceptionHandlerBinder
    {
        internal static void Bind()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        public static void Unbind()
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException -= Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ExceptionInfoFormatFriendly(e.Exception, e.ToString());
            e.SetObserved();
        }

        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionInfoFormatFriendly(e.Exception, e.ToString());
            e.Handled = true;
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionInfoFormatFriendly(e.ExceptionObject as Exception, e.ToString());
        }

        /// <summary>h
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        private static string ExceptionInfoFormatFriendly(Exception ex, string backStr)
        {
            var str = string.Empty;

            #region Obsolete.
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("****************************异常文本****************************");
            //sb.AppendLine("【出现时间】：" + DateTime.Now);
            //if (ex != null)
            //{
            //    sb.AppendLine("【异常类型】：" + ex.GetType().Name);
            //    sb.AppendLine("【异常信息】：" + ex.Message);
            //    sb.AppendLine("【堆栈调用】：" + ex.StackTrace);

            //    sb.AppendLine("【异常方法】：" + ex.TargetSite);

            //}
            //else
            //{
            //    sb.AppendLine("【未处理异常】：" + backStr);
            //}
            //sb.AppendLine("***************************************************************");
            //str = sb.ToString(); 
            #endregion

            Tracer.Error(ex, "全局异常处理");
            return str;
        }
    }
}
