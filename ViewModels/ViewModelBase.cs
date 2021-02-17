using Dental.Models.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dental.ViewModels
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChange([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChange(PropertyName);
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            _Disposed = true;
        }
    }
}
