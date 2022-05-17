using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;

namespace Dental.Services
{
    public class UpdateRowService : ServiceBase, IUpdateRowService
    {
        public void UpdateRow() => (AssociatedObject as TableView).UpdateRow();
      
    }
}
