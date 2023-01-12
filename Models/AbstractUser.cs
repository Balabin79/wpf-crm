using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;
using Dental.Services.Files;
using System.Windows.Media;
using System.Text.Json.Serialization;

namespace Dental.Models
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

        public byte[] Img
        {
            get { return GetProperty(() => Img); }
            set { SetProperty(() => Img, value); }
        }

        [NotMapped]
        public ImageSource Image
        {
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

        [NotMapped]
        public string Photo
        {
            get { return GetProperty(() => Photo); }
            set { SetProperty(() => Photo, value); }
        }
      
        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
