using Dental.Models.Base;
using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Windows.Input;

namespace Dental.Models
{
    [Table("Address")]
    class Address : AbstractBaseModel
    {
        [Display(Name = "Страна")]
        public int? CountryId { get; set; }

        [Display(Name = "Область, край, республика")]
        public int? RegionId { get; set; }

        [Display(Name = "Район")]
        public int? AreaId { get; set; }

        [Display(Name = "Населенный пункт")]
        public int? CityId { get; set; }

        [Display(Name = "Улица")]
        public string Street { get; set; }

        [Display(Name = "Дом")]
        public string House { get; set; }

        [Display(Name = "Корпус")]
        public string Housing { get; set; }

        [Display(Name = "Квартира")]
        public string Apartment { get; set; }


        public bool this[PropertyInfo prop, Address item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "CountryId": return item.CountryId == CountryId;
                    case "RegionId": return item.RegionId == RegionId;
                    case "AreaId": return item.AreaId == AreaId;                 
                    case "CityId": return item.CityId == CityId;                 
                    case "Street": return item.Street == Street;                 
                    case "House": return item.House == House;                 
                    case "Housing": return item.Housing == Housing;                 
                    case "Apartment": return item.Apartment == Apartment;                               
                    default: return true;
                }
            }
        }

        public void Copy(Address copy)
        {
            CountryId = copy.CountryId;
            RegionId = copy.RegionId;
            AreaId = copy.AreaId;
            CityId = copy.CityId;
            Street = copy.Street;
            House = copy.House;
            Housing = copy.Housing;
            Apartment = copy.Apartment;
        }
    }
}
