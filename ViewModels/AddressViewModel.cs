using System.Linq;
using Dental.Models;
using System.Collections.Generic;
using Dental.Models.Share;
using MoreLinq;

namespace Dental.ViewModels
{
    class AddressViewModel : ViewModelBase
    {    
        public AddressViewModel(Employee employee)
        {
            db = new ApplicationContext();
            countries = db.Country.OrderBy(f => f.TitleRu).ToList();

            Employee = employee;
            SelectedCountry = (Employee.CountryId != null ) ? db.Country.Where(f => f.CountryId == Employee.CountryId).FirstOrDefault() : null;
            SelectedRegion = (Employee.RegionId != null) ? db.Region.Where(f => f.RegionId == Employee.RegionId).FirstOrDefault() : null;
            SelectedArea = !string.IsNullOrEmpty(Employee.Area) && Employee.RegionId != null ? db.City.Where(f => f.AreaRu.Contains(Employee.Area) && f.RegionId == Employee.RegionId).FirstOrDefault() : null;
            SelectedLocality = Employee.CityId != null && Employee.RegionId != null ? db.City.Where(f => f.CityId == Employee.CityId).FirstOrDefault() : null;
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
            set{ Set(ref regions, value); }
        }

        private IEnumerable<City> area;
        public IEnumerable<City> Area
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

        private IEnumerable<City> locality;
        public IEnumerable<City> Locality
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
                if (selectedCountry == null) return;
                Employee.CountryId = ((Country)selectedCountry).CountryId;
                Regions = db.Region.Where(f => f.CountryId == ((Country)selectedCountry).CountryId)
                    .OrderBy(f => f.TitleRu).ToList();

                SelectedRegion = null;
                SelectedArea = null;
                SelectedLocality = null;

                IsEnabledRegionField = true;
                IsEnabledAreaField = false;
                IsEnabledLocalityField = false;         
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
                Employee.RegionId = ((Region)selectedRegion).RegionId;
                SelectedArea = null;
                SelectedLocality = null;
                Locality = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu == "")
                    .DistinctBy(f => f.TitleRu)
                    .OrderBy(f => f.TitleRu)
                    .ToList(); 

                Area = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu != ""
                && f.AreaRu.Contains(" район"))
                    .DistinctBy(f => f.AreaRu)   
                    .OrderBy(f => f.AreaRu)
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
                if (selectedArea == null || selectedRegion == null) return;
                Employee.Area = ((City)selectedArea).AreaRu;

                SelectedLocality = null;

                Locality = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId &&
                     f.AreaRu != "" && f.AreaRu.Contains(((City)selectedArea).AreaRu))
                    .DistinctBy(f => f.TitleRu)
                    .OrderBy(f => f.TitleRu)
                    .ToList();
            }
        }

        private object selectedLocality;
        public object SelectedLocality 
        { 
            get => selectedLocality;
            set
            {
                Set(ref selectedLocality, value);
                if (selectedLocality == null) return;
                Employee.CityId = ((City)selectedLocality).CityId;
            } 
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
