using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface ITreeModel<T> :  IModel, ITree, ICloneable 
    {
        T Parent { get; set; }
    }
}
