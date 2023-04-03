using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Models.Base
{
    public interface ITree
    {
        int? IsDir { get; set; }
        int? ParentID { get; set; }
        string Name { get; set; }
    }
}
