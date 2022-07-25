using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Settings : AbstractBaseModel
    {
        [Required(ErrorMessage = @"���� ""������� Google"" ����������� ��� ����������")]
        [MaxLength(255, ErrorMessage = @"������������ ����� ������ � ���� ""������� Google"" �� ����� 255 ��������")]
        [EmailAddress(ErrorMessage = @"� ���� ""������� Google"" ������� ������������ ��������")]
        public string GoogleAccount { get; set; }
        public int? IsUseGoogleIntegration { get; set; } = 0;

        public int? UpdateInterval { get; set; } = 5;

        public string ContactGroupForEmployees { get; set; } = "�������� �����������";
        public string ContactGroupForClients { get; set; } = "�������� ��������";
        public string DirectoryNameForEmployeeFiles { get; set; } = "����� �����������";
        public string DirectoryNameForClientFiles { get; set; } = "����� ��������";

        public string GetFieldsValues()
        {
            return "";
            //GetHashCode GoogleAccount
        }
    }
}
