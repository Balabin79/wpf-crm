using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Share
{
    [Table("City")]
    public class City 
    {
        [Column("country_id")]
        public int? CountryId { get; set; }

        [Column("region_id")]
        public int? RegionId { get; set; }

        [Column("city_id")]
        public int? CityId { get; set; }

        [Column("title_ru")]
        public string TitleRu { get; set; }

        [Column("area_ru")]
        public string AreaRu { get; set; }

        [Column("region_ru")]
        public string RegionRu { get; set; }
    }
}
