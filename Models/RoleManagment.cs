using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("RolesManagment")]
    public class RoleManagment : AbstractBaseModel
    {

        public string PageName { get; set; }
        public string PageTitle { get; set; }

        public int? DoctorAccessId { get; set; }
        public Access DoctorAccess { get; set; }

        public int? AdminAccessId { get; set; }
        public Access AdminAccess { get; set; }

        public int? ReceptionAccessId { get; set; }
        public Access ReceptionAccess { get; set; }
    }
}
