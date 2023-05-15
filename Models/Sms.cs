using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace B6CRM.Models
{
    [Table("Sms")]
    public class Sms : AbstractBaseModel
    {
        public Sms() => SmsRecipients = new ObservableCollection<SmsRecipient>();
        
        public string Name { get; set; }
        public string Date { get; set; }
        public string Msg { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Channel Channel { get; set; }
        public int? ChannelId { get; set;}

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public ClientCategory ClientCategory { get; set; }
        public int? ClientCategoryId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public SendingStatus SendingStatus { get; set; }
        public int? SendingStatusId { get; set; }

        public int ServiceId { get; set; }

        public ObservableCollection<SmsRecipient> SmsRecipients
        {
            get { return GetProperty(() => SmsRecipients); }
            set { SetProperty(() => SmsRecipients, value); }
        }
    }
}
