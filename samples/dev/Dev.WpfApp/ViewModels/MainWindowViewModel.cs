using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dev.WpfApp.Views;
using MahApps.Metro.IconPacks;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

namespace Dev.WpfApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {
        private static readonly ObservableCollection<MenuItem> AppMenu = new ObservableCollection<MenuItem>();
        private static readonly ObservableCollection<MenuItem> AppOptionsMenu = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Menu => AppMenu;

        public ObservableCollection<MenuItem> OptionsMenu => AppOptionsMenu;

        [ObservableProperty]
        [Required]
        [MinLength(6)]
        private string? _title;

        [ObservableProperty]
        private ObservableCollection<TitleVm> _items;
        private Size _size;
        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }
        public DateTime CreatedAt { get; private set; }

        public MainWindowViewModel()
        {
            Title = "主窗体";
            _size = new Size(1366, 768);
            _items = new ObservableCollection<TitleVm>() { new TitleVm { Id = 1, Title = Title, CreatedAt = DateTime.Now } };
            var len = 10000;
            while (len-- > 0)
            {
                var id = Items.Count + 1;
                _items.Insert(0, new TitleVm() { Id = id, Title = $"{Title}_{id}", CreatedAt = DateTime.Now });
            }

            MenuInit();
        }


        [RelayCommand]
        public void ModifyTitle(string @new)
        {
            Title = @new;
            var id = Items.Count + 1;
            _items.Insert(0, new TitleVm() { Id = id, Title = $"{Title}_{id}", CreatedAt = DateTime.Now });
        }

        [RelayCommand]
        public void DeleteTitle(int id)
        {
            if (!Items.Any(i => i.Id == id))
                return;
            Items.Remove(Items.First(i => i.Id == id));
        }

        [RelayCommand]
        public void ChangeSize()
        {
            Size = new Size(1024,768);
        }

        [RelayCommand]
        public void ChangeTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(Theme.Light);
            theme.SetPrimaryColor(Colors.DarkBlue);
            theme.SetSecondaryColor(Colors.Gray);
            //theme.PrimaryMid = new MaterialDesignColors.ColorPair(Colors.Brown, Colors.White);
            paletteHelper.SetTheme(theme);
        }
        private void MenuInit()
        {
            // Build the menus
            Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.BugSolid },
                Label = "Bugs",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/BugsPage.xaml", UriKind.RelativeOrAbsolute)
            });
            Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.UserSolid },
                Label = "User",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/UserPage.xaml", UriKind.RelativeOrAbsolute)
            });
            Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CoffeeSolid },
                Label = "Break",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/BreakPage.xaml", UriKind.RelativeOrAbsolute)
            });
            Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.FontAwesomeBrands },
                Label = "Awesome",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/AwesomePage.xaml", UriKind.RelativeOrAbsolute)
            });

            OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogsSolid },
                Label = "Settings",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });
            OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoCircleSolid },
                Label = "About",
                NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Views/AboutPage.xaml", UriKind.RelativeOrAbsolute)
            });
        }
    }

    public class TitleVm
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
