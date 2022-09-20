using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    interface ICategoryTree
    {
        int? IsCategory { get; set; }
        int? ParentId { get; set; }
    }
}
