using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Table("UserTemplates")]
    public class UserTemplate : BaseTemplate<UserTemplate>
    {

        public byte[] Img
        {
            get { return GetProperty(() => Img); }
            set { SetProperty(() => Img, value); }
        }

        public override object Clone()
        {
            return new UserTemplate() 
            {
                IsDir = IsDir, 
                Name = Name, 
                Parent = Parent, 
                ParentID = ParentID,
                UpdatedAt = UpdatedAt 
            };
        }
    }
}
