using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Share
{
    [Table("Country")]
    class Country
    {
        [Column("country_id")]
        public int? CountryId { get; set; }

        [Column("title_ru")]
        public string TitleRu { get; set; }
    }
}
