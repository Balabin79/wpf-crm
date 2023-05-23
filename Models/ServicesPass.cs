using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("ServicePasses")]
    public class ServicePass : AbstractBaseModel
    {
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Login
        {
            get { return GetProperty(() => Login); }
            set { SetProperty(() => Login, value); }
        }

        public string Pass
        {
            get { return GetProperty(() => Pass); }
            set { SetProperty(() => Pass, value); }
        }

        public string SenderName
        {
            get { return GetProperty(() => SenderName); }
            set { SetProperty(() => SenderName, value); }
        }

        public int? IsCascadeRoutingEnabled
        {
            get { return GetProperty(() => IsCascadeRoutingEnabled); }
            set { SetProperty(() => IsCascadeRoutingEnabled, value); }
        }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public TimeZone TimeZone
        {
            get { return GetProperty(() => TimeZone); }
            set { SetProperty(() => TimeZone, value); }
        }

        public int? TimeZoneId
        {
            get { return GetProperty(() => TimeZoneId); }
            set { SetProperty(() => TimeZoneId, value); }
        }

        [NotMapped]
        public string PassDecr
        {
            get { return GetProperty(() => PassDecr); }
            set { SetProperty(() => PassDecr, value); }
        }
    }
}
