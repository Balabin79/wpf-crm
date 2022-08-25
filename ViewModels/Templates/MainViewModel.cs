using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
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
            NavigationService.Navigate("MainPage", null, this);
        }

   
        public void NavigateToDiagnoses()
        {
            //NavigationService.Navigate("Dental.Views.TemplateForms.DiagnosPage", null, this);
        }


        public void NavigateToDiaries()
        {
            //NavigationService.Navigate("NextDetailView", null, this);
        }

       
        public void NavigateToTreatmentPlans()
        {
           // NavigationService.Navigate("NextDetailView", null, this);
        }

        [Command]
        public void NavigateToInitialInspections()
        {
            //NavigationService.Navigate("NextDetailView", null, this);
        }

        [Command]
        public void NavigateBack()
        {
           // NavigationService.GoBack();
        }

        [Command]
        public bool CanNavigateBack()
        {
            return NavigationService.CanGoBack;
        }

        [Command]
        public void NavigateForward()
        {
            //NavigationService.GoForward();
        }

        [Command]
        public bool CanNavigateForward()
        {
            return NavigationService != null && NavigationService.CanGoForward;
        }
    }
}
