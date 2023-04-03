using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Reports
{
    public class PrintCondition
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Type Type { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
