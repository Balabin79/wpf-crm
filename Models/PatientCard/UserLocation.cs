using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.PatientCard
{
    [Table("UserLocation")]
    public class UserLocation : AbstractBaseModel
    {
        [Required]
        public int UserId { get; set; } 
        public int? CountryId { get; set; } 
        public int? RegionId { get; set; } // область, край, республика 
        public int? DistrictId { get; set; } // район
        public int? CityId { get; set; }

        [Required]
        public string LocalityName { get; set; } // Название населенного пункта


        public string Street { get; set; } 
        public string House { get; set; } 

        public string Apartment { get; set; } // Квартира
    }
}
