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

namespace Dental.ViewModels
{
    class AddressViewModel : ViewModelBase
    {

        public AddressViewModel()
        {
            db = new ApplicationContext();  
            Regions = db.Region.Where(f => f.CountryId == 0).ToList();
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
            set => Set(ref regions, value); 
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


        private object selectedCountry;
        public object SelectedCountry
        {
            get => selectedCountry;
            set
            {
                Set(ref selectedCountry, value);
                Regions = db.Region.Where(f => f.CountryId == ((Country)selectedCountry).CountryId)
                    .OrderBy(f => f.TitleRu).ToList();
            }
        }

        private object selectedRegion;
        public object SelectedRegion
        {
            get => selectedRegion;
            set
            {
                Set(ref selectedRegion, value);
                Cities = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu == "").
                    OrderBy(f => f.AreaRu).ToList();

                Area = db.City.Where(f => f.RegionId == ((Region)selectedRegion).RegionId && f.AreaRu != ""
                && f.AreaRu.Contains(" район"))
                    .Select(f => f.AreaRu)
                    .Distinct()   
                    .OrderBy(f => f)
                    .ToList();
            }
        }

        private object selectedArea;
        public object SelectedArea
        {
            get => selectedArea;
            set
            {
                Set(ref selectedArea, value);
                Cities = db.City.Where(f => f.CountryId == ((Country)selectedCountry).CountryId).ToList();
            }
        }
    }
}
