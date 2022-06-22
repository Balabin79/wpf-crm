using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Infrastructures.Converters
{
    public class DocParams
    {
        public BarButtonItem Item { get; set; }
        public string PathToFile { get; set; } 
        public Type DocType { get; set; } 
    }
}
