using Dev.WpfApp.Navigation;
using MahApps.Metro.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using MenuItem = Dev.WpfApp.ViewModels.MenuItem;

namespace Dev.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly NavigationServiceEx navigationServiceEx;
        private NotifyIcon? _notifyIcon;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            if (Tag is null)
            {
                Tag = true;

                navigationServiceEx = new NavigationServiceEx();
                navigationServiceEx.Navigated += NavigationServiceEx_OnNavigated;
                HamburgerMenuControl.Content = navigationServiceEx.Frame;
                // Navigate to the home page.
                Loaded += (sender, args) => navigationServiceEx.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));

                _notifyIcon = new NotifyIcon();
                _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                _notifyIcon.Visible = true;
                _notifyIcon.Text = "Kowalski";

                var cms = new ContextMenuStrip();
                cms.Items.Add("关于", null, (s, e) =>
                {
                    _notifyIcon.BalloonTipText = "Welcome to KOWALSKI.";
                    _notifyIcon.BalloonTipTitle = "Kowalski";
                    _notifyIcon.ShowBalloonTip(1000);
                });
                cms.Items.Add("退出", null, (s, e) => App.Current.Shutdown());
                _notifyIcon.ContextMenuStrip = cms;
            }
        }

        private void DeployCupCakes(object sender, RoutedEventArgs e)
        {
            // deploy some CupCakes...
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is MenuItem menuItem && menuItem.IsNavigation)
            {
                this.navigationServiceEx.Navigate(menuItem.NavigationDestination);
            }
        }

        private void NavigationServiceEx_OnNavigated(object sender, NavigationEventArgs e)
        {
            // select the menu item
            this.HamburgerMenuControl.SelectedItem = this.HamburgerMenuControl
                                                         .Items
                                                         .OfType<MenuItem>()
                                                         .FirstOrDefault(x => x.NavigationDestination == e.Uri);
            this.HamburgerMenuControl.SelectedOptionsItem = this.HamburgerMenuControl
                                                                .OptionsItems
                                                                .OfType<MenuItem>()
                                                                .FirstOrDefault(x => x.NavigationDestination == e.Uri);

            // or when using the NavigationType on menu item
            // this.HamburgerMenuControl.SelectedItem = this.HamburgerMenuControl
            //                                              .Items
            //                                              .OfType<MenuItem>()
            //                                              .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());
            // this.HamburgerMenuControl.SelectedOptionsItem = this.HamburgerMenuControl
            //                                                     .OptionsItems
            //                                                     .OfType<MenuItem>()
            //                                                     .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());

            // update back button
            this.GoBackButton.Visibility = this.navigationServiceEx.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            this.navigationServiceEx.GoBack();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_notifyIcon is null)
                return;

            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}