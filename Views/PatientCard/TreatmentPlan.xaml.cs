using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;

namespace Dental.Views.PatientCard
{
    public partial class TreatmentPlan : UserControl
    {
        public TreatmentPlan()
        {
            InitializeComponent();
        }

        private void GridControlPriceForClients_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int x = 0;
        }

        private void lookup_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {

        }


    }
}
