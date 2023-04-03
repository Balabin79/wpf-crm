using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Models.Base
{
    interface ICategoryTree
    {
        int? IsCategory { get; set; }
        int? ParentId { get; set; }
    }
}
