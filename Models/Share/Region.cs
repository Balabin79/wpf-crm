using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Share
{
    [Table("Region")]
    class Region
    {
        [Column("country_id")]
        public int? CountryId { get; set; }

        [Column("region_id")]
        public int? RegionId { get; set; }

        [Column("title_ru")]
        public string TitleRu { get; set; }
    }
}
