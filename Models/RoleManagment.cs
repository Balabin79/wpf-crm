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
    public class RoleManagment : AbstractBaseModel, ICategoryTree
    {

        public string PageName { get; set; }
        public string PageTitle { get; set; }

        public int? DoctorAccess { get; set; } = 0;

        public int? AdminAccess { get; set; } = 0;

        public int? ReceptionAccess { get; set; } = 0;

        public int? ParentId { get; set; }
        public int? IsCategory { get; set; }

        public int? Num { get; set; }
    }
}
