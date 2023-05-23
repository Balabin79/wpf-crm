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
        public Sms() 
        { 
            SmsRecipients = new ObservableCollection<SmsRecipient>();
            SmsSendingDate = new ObservableCollection<SmsSendingDate>();
        }
        
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public string Msg
        {
            get { return GetProperty(() => Msg); }
            set { SetProperty(() => Msg, value); }
        }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Channel Channel { get; set; }
        public int? ChannelId { get; set;}

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public ClientCategory ClientCategory { get; set; }
        public int? ClientCategoryId { get; set; }

        public int ServiceId { get; set; }

        public ObservableCollection<SmsRecipient> SmsRecipients
        {
            get { return GetProperty(() => SmsRecipients); }
            set { SetProperty(() => SmsRecipients, value); }
        }

        public ObservableCollection<SmsSendingDate> SmsSendingDate
        {
            get { return GetProperty(() => SmsSendingDate); }
            set { SetProperty(() => SmsSendingDate, value); }
        }
    }
}
