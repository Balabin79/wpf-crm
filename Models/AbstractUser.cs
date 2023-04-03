using System.ComponentModel.DataAnnotations.Schema;
using System;
using B6CRM.Models.Base;

namespace B6CRM.Models
{
    public abstract class AbstractUser : AbstractBaseModel
    {

        public string FirstName
        {
            get { return GetProperty(() => FirstName); }
            set { SetProperty(() => FirstName, value?.Trim()); }
        }

        public string LastName
        {
            get { return GetProperty(() => LastName); }
            set { SetProperty(() => LastName, value?.Trim()); }
        }

        public string MiddleName
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, value?.Trim()); }
        }

        [NotMapped]
        public string FullName { get => ToString(); }

        [NotMapped]
        public string ShortName
        {
            get
            {
                string str = LastName;
                if (!string.IsNullOrEmpty(FirstName)) str += " " + FirstName?.Substring(0, 1)?.ToUpper() + ".";
                if (!string.IsNullOrEmpty(MiddleName)) str += MiddleName?.Substring(0, 1)?.ToUpper() + ".";
                return str;
            }
        }

        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value); }
        }

        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
