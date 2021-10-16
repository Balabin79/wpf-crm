using Dental.Models.Base;
using Dental.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    class ClientFiles : ViewModelBase
    {
        public ClientFiles()
        {
            Status = STATUS_NEW_RUS;
        }


        public const string STATUS_SAVE_RUS = "��������";
        public const string STATUS_NEW_RUS = "�� ��������";

        public int Id { get; set; }
        public string Name { get; set; } = "��� ��������";
        public string FullName { get; set; } = "��� ��������";
        public string Path { get; set; } = "";
        public string DateCreated { get; set; }
        public string Extension { get; set; } = "";
        public string Size { get; set; }

        public string _Status;
        public string Status {
            get => _Status;
            set => Set(ref _Status, value);
        }
    }
}
