using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Settings : AbstractBaseModel
    {
        public string KeyName { get; set; } = "google_account";
        public string Key { get; set; }
        public string Value { get; set; }

        public ISettings Copy(ISettings settings)
        {
            settings.Key = Key;
            settings.Value = Value;
            return settings;
        }
    }
}
