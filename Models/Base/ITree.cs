using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface ITree
    {
        int? IsDir { get; set; }
        int? ParentId { get; set; }             
        string Name { get; set; }
    }
}
