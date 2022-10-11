using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels
{
    public class BaseWrapperViewModel<T> : ViewModelBase
    {
        public virtual T Model { get; set; }
        public virtual T Copy { get; set; }
    }
}
