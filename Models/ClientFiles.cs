using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientFiles")]
    class ClientFiles : AbstractBaseModel
    {
        public const string STATUS_SAVE_RUS = "Сохранен";
        public const string STATUS_NEW_RUS = "Не сохранен";

        public string Name { get; set; } = "Без названия";
        public string Path { get; set; } = "";
        public string DateCreated { get; set; }
        public string Extension { get; set; } = "";
        public string Size { get; set; }
        public string Status { get; set; } = STATUS_NEW_RUS;
    }
}
