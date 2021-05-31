using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    interface ITree
    {
        int IsDir { get; set; }
        int? ParentId { get; set; }
    }
}
