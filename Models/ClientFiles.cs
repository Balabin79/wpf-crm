using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientFiles")]
    class ClientFiles : AbstractBaseModel
    {
        public const string STATUS_SAVE_RUS = "��������";
        public const string STATUS_NEW_RUS = "�� ��������";

        public string Name { get; set; } = "��� ��������";
        public string Path { get; set; } = "";
        public string DateCreated { get; set; }
        public string Extension { get; set; } = "";
        public string Size { get; set; }
        public string Status { get; set; } = STATUS_NEW_RUS;
    }
}
