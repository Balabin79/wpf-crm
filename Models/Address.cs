using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

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
        public string Area { get; set; }

        [Display(Name = "Населенный пункт")]
        public string Locality { get; set; }

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
                    case "AreaId": return item.Area == Area;                 
                    case "CityId": return item.Locality == Locality;                 
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
            Area = copy.Area;
            Locality = copy.Locality;
            Street = copy.Street;
            House = copy.House;
            Housing = copy.Housing;
            Apartment = copy.Apartment;
        }
    }
}
