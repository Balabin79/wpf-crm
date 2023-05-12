using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("Channels")]
    public class Channel : AbstractBaseModel
    {
        public string Name { get; set; }
        public int ProstoSms { get; set; }
        public int SmsCenter { get; set; }
        public int Unisender { get; set; }
    }
}
