using Dental.Models;
using Dental.Models.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels
{
    class AddressWidgetViewModel : ViewModelBase
    {
        public AddressWidgetViewModel() : this(0,0,"",0) {}
        public AddressWidgetViewModel(int selectedCountryId) : this(selectedCountryId, 0){}
        public AddressWidgetViewModel(int selectedCountryId, int selectedRegionId) : this(selectedCountryId, selectedRegionId, "") { }
        public AddressWidgetViewModel(int selectedCountryId, int selectedRegionId, string selectedArea) : this(selectedCountryId, selectedRegionId, "", 0) { }

        public AddressWidgetViewModel(int selectedCountryId, int selectedRegionId, string selectedArea, int selectedLocalityId)
        {
            db = new ApplicationContext();
            Countries = db.Country.OrderBy(f => f.CountryId).ToList();

            SelectedCountry = selectedCountryId > 0 ? db.Country.Where(f => selectedCountryId == f.CountryId).FirstOrDefault() : null;
            SelectedRegion = selectedRegionId > 0 ? db.Region.Where(f => selectedRegionId == f.RegionId).FirstOrDefault() : null;
            SelectedArea = !string.IsNullOrEmpty(selectedArea) && selectedRegionId > 0 ? db.City.Where(f => f.AreaRu.Contains(selectedArea) && f.RegionId == selectedRegionId).FirstOrDefault() : null;
            SelectedLocality = selectedLocalityId > 0 && selectedRegionId > 0 ? db.City.Where(f => f.CityId == selectedLocalityId).FirstOrDefault() : null;
        }


        public string FullAddress { get; set; }

        public object SelectedCountry
        {
            get => _SelectedCountry;
            set
            {/*
                try
                {
                    Set(ref _SelectedCountry, value);
                    if (_SelectedCountry == null) return;

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
                catch (Exception ex)
                {
                    setDefaultValuesOnExceptions();
                }*/

            }
        }
        public object SelectedRegion { get; set; }
        public object SelectedArea { get; set; }
        public object SelectedLocality { get; set; }


        public IEnumerable<Country> Countries { get; set; }

        public IEnumerable<Region> Regions
        {
            get => _Regions;
            set => Set(ref _Regions, value);
        }

        public IEnumerable<City> Area
        {
            get => _Areas;
            set => Set(ref _Areas, value);
        }

        public IEnumerable<City> Locality
        {
            get => _Localities;
            set => Set(ref _Localities, value);
        }

        public bool IsEnabledRegionField
        {
            get => _IsEnabledRegionField;
            set => Set(ref _IsEnabledRegionField, value);
        }

        public bool IsEnabledAreaField
        {
            get => _IsEnabledAreaField;
            set => Set(ref _IsEnabledAreaField, value);
        }

        public bool IsEnabledLocalityField
        {
            get => _IsEnabledLocalityField;
            set => Set(ref _IsEnabledLocalityField, value);
        }


        private readonly ApplicationContext db;
        private object _SelectedCountry;
        private object _SelectedRegion;
        private object _SelectedArea;
        private object _SelectedLocality;
        private Country _Countries;
        private IEnumerable<Region> _Regions;
        private IEnumerable<City> _Areas;
        private IEnumerable<City> _Localities;
        private bool _IsEnabledRegionField = false;
        private bool _IsEnabledAreaField = false;
        private bool _IsEnabledLocalityField = false;

        private void setRegionFieldByCountryId()
        {

        }

    }
}
