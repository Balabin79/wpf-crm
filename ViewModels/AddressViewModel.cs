using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Repositories;
using DevExpress.Xpf.Grid;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Windows;
using Dental.Models.Share;
using DevExpress.Mvvm.UI;
using System.IO;
using System.Reflection;
using DevExpress.Mvvm.POCO;

namespace Dental.ViewModels
{
    class AddressViewModel : ViewModelBase
    {

        public AddressViewModel()
        {
            db = new ApplicationContext();

            Regions = null;
            Cities = null;
            Area = null;
            Locality = null;
        }

        private readonly ApplicationContext db;
        

        public IEnumerable<Country> Countries 
        { 
            get => db.Country.OrderBy(f => f.CountryId).ToList(); 
        }

        private IEnumerable<Region> regions;
        public IEnumerable<Region> Regions
        {
            get => regions; 
            set{ Set(ref regions, value); }
        }

        private IEnumerable<string> area;
        public IEnumerable<string> Area
        {
            get => area;
            set => Set(ref area, value);
        }

        private IEnumerable<City> cities;
        public IEnumerable<City> Cities
        {
            get => cities;
            set => Set(ref cities, value);
        }

        private IEnumerable<string> locality;
        public IEnumerable<string> Locality
        {
            get => locality;
            set => Set(ref locality, value);
        }


        private object selectedCountry;
        public object SelectedCountry
        {
            get => selectedCountry;
            set
            {
                Set(ref selectedCountry, value);

                Regions = null;

                Regions = db.Region.Where(f => f.CountryId == ((Country)selectedCountry).CountryId)
                    .OrderBy(f => f.TitleRu).ToList();

                Area = null;
                Locality = null;

                SelectedRegion = null;
                SelectedArea = null;
                SelectedLocality = null;

                IsEnabledRegionField = true;
                IsEnabledAreaField = false;
                IsEnabledLocalityField = false;


               var notificationService = ServiceContainer.Default.GetService<INotificationService>("NotificationService");
               
                var sri = Application.GetResourceStream(
                    new Uri(@"pack://application:,,,/Dental;component/Resources/Sounds/Notifications/success.wav")
                    );
                if ((sri != null))
                {
                    using (var s = sri.Stream)
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(s);
                        player.Load();
                        player.Play();
                    }
                }
                 //notificationService.CreatePredefinedNotification("Уведомление", "Новый сотрудник успешно записан в базу данных!", "", ImageNotification).ShowAsync();
               
                notificationService.CreateCustomNotification(this).ShowAsync();
            }
        }

        public string Caption { get; set; } = "Уведомление";
        public string Content { get; set; } = "Новый сотрудник успешно записан в базу данных!";
 

        public System.Windows.Media.ImageSource ImageNotification
        {
            get 
            {
               var img = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/Gnome-Document-Save-64.png"));
                return img;
            } 
        }

        private object selectedRegion;
        public object SelectedRegion
        {
            get => selectedRegion;
            set
            {
                Set(ref selectedRegion, value);
                if (selectedRegion == null) return;

                SelectedArea = null;
                SelectedLocality = null;

                Area = null;
                Locality = null;

                Locality = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu == "")
                    .Select(f => f.TitleRu)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList(); 

                Area = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu != ""
                && f.AreaRu.Contains(" район"))
                    .Select(f => f.AreaRu)
                    .Distinct()   
                    .OrderBy(f => f)
                    .ToList();

                IsEnabledRegionField = true;
                IsEnabledAreaField = true;
                IsEnabledLocalityField = true;
            }
        }

        private object selectedArea;
        public object SelectedArea
        {
            get => selectedArea;
            set
            {
                Set(ref selectedArea, value);
                if (selectedArea == null) return;
                var areaName = SelectedArea?.ToString() ?? "";
                Locality = null;
                SelectedLocality = null;
                Locality = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId &&
                f.AreaRu != "" && f.AreaRu.Contains(areaName))
                    .Select(f => f.TitleRu)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList();
            }
        }

        private object selectedLocality;
        public object SelectedLocality 
        { 
            get => selectedLocality;
            set => Set(ref selectedLocality, value); 
        }

        private bool isEnabledRegionField = false;
        public bool IsEnabledRegionField 
        {
            get => isEnabledRegionField;
            set => Set(ref isEnabledRegionField, value);
        }

        private bool isEnabledAreaField = false;
        public bool IsEnabledAreaField
        {
            get => isEnabledAreaField;
            set => Set(ref isEnabledAreaField, value);
        }

        private bool isEnabledLocalityField = false;
        public bool IsEnabledLocalityField
        {
            get => isEnabledLocalityField;
            set => Set(ref isEnabledLocalityField, value);
        }




    }
}
