using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public class TreeTemplate : ITree, IModel
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; } = false;
        public int? IsDir { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }

        public int? CreatedAt { get; set; }
        public int? UpdatedAt { get; set; }
        public string Guid { get; set; }
    }
}
