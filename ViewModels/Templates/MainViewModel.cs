using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels.Templates
{
    class MainViewModel : ViewModelBase
    {
        private INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }

        public MainViewModel() { }

        public void OnViewLoaded()
        {
            NavigationService.Navigate("HomeView", null, this);
        }
    }
}
