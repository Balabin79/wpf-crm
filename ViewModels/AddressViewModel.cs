using System.Linq;
using Dental.Models;
using System.Collections.Generic;
using MoreLinq;
using System;
using System.Windows;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class AddressViewModel : ViewModelBase
    {    
        public AddressViewModel(Employee employee)
        {
            try { 
                db = new ApplicationContext();
                countries = db.Country.OrderBy(f => f.CountryId).ToList();

                Employee = employee;
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом адресов!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
            // SelectedCountry = (Employee.CountryId != null ) ? db.Country.Where(f => f.CountryId == Employee.CountryId).FirstOrDefault() : null;
            // SelectedRegion = (Employee.RegionId != null) ? db.Region.Where(f => f.RegionId == Employee.RegionId).FirstOrDefault() : null;
        }

        private Employee Employee { get; set; }
        private readonly ApplicationContext db;
        private readonly IEnumerable<Country> countries;

        public IEnumerable<Country> Countries 
        {
            get => countries;
        }

        private IEnumerable<Region> regions;
        public IEnumerable<Region> Regions
        {
            get => regions; 
            set => Set(ref regions, value);
        }



        private object selectedCountry;
        public object SelectedCountry
        {
            get => selectedCountry;
            set
            {
                try
                {
                    Set(ref selectedCountry, value);
                    if (selectedCountry == null) return;
                    //Employee.CountryId = ((Country)selectedCountry).CountryId;
                    Regions = db.Region.Where(f => f.CountryId == ((Country)selectedCountry).CountryId)
                        .OrderBy(f => f.TitleRu).ToList();

                    SelectedRegion = null;

                    IsEnabledRegionField = true;
                    IsEnabledAreaField = false;
                    IsEnabledLocalityField = false;
                } catch (Exception ex)
                {
                    setDefaultValuesOnExceptions();
                }
        
            }
        }

        private object selectedRegion;
        public object SelectedRegion
        {
            get => selectedRegion;
            set
            {
                try
                {
                    Set(ref selectedRegion, value);
                    if (selectedRegion == null) return;
                    //Employee.RegionId = ((Region)selectedRegion).RegionId;

                    IsEnabledRegionField = true;
                    IsEnabledAreaField = true;
                    IsEnabledLocalityField = true;
                } catch(Exception ex)
                {
                    setDefaultValuesOnExceptions();
                }
            }
        }


        public object CustomLocality { get; set; } // добавить населенный пункт
        public object DefaultAddress { get; set; } // подгружать населенные пункты по-умолчанию

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

        private void setDefaultValuesOnExceptions()
        {
            SelectedRegion = null;

            IsEnabledRegionField = false;
            IsEnabledAreaField = false;
            IsEnabledLocalityField = false;
        }
    }
}
