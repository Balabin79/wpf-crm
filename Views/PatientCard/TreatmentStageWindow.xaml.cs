using System.Windows;

namespace Dental.Views.PatientCard
{
    public partial class TreatmentStageWindow : Window
    {
        public TreatmentStageWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
