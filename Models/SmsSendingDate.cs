using B6CRM.Models.Base;
using DevExpress.CodeParser;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("SmsSendingDate")]
    public class SmsSendingDate : AbstractBaseModel
    {
        public string Date { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Sms Sms { get; set; }       
        public int? SmsId { get; set; }   
        
        public int? IDSms { get; set; }

        public int? Count { get; set; }

        public string ChannelName { get; set; }

        [Column(TypeName = "NUMERIC")]
        public decimal? Cost
        {
            get { return GetProperty(() => Cost); }
            set { SetProperty(() => Cost, value); }
        }
    }

}
