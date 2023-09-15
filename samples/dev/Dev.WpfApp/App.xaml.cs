using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Dev.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _initialized;
        public App()
        {
            InitializeComponent();
            if (!_initialized)
            {
                _initialized = true;

                Ioc.Default.AddCore(services=>ConfigureServices(services));//Register exception handlers.
            }
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var shell = Ioc.Default.GetRequiredService<MainWindow>();
            shell.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Ioc.Default.UnbindExceptionHandler();//Unregister exception handlers.
        }
    }
}
