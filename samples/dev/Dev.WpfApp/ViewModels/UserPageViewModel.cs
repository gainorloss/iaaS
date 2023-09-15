using CommunityToolkit.Mvvm.ComponentModel;
using Dev.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Dev.WpfApp.ViewModels
{
    internal partial class UserPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserOutput>? _users;

        public UserPageViewModel()
        {
            var usrs = new List<UserOutput>
            {
                new UserOutput { Age = 30, Gender = Gender.Male.ToString() },
                new UserOutput { Age = 29, Gender = Gender.Female.ToString() }
            };
            _users = new ObservableCollection<UserOutput>(usrs);
        }
    }
}
