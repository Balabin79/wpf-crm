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
        public virtual T Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public virtual T Copy
        {
            get { return GetProperty(() => Copy); }
            set { SetProperty(() => Copy, value); }
        }
    }
}
