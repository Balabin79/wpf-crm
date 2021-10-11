using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System.Windows.Controls;

namespace Dental.Views.PatientCard
{

    public partial class MainInfoPage : Page
    {
        public MainInfoPage()
        {
            InitializeComponent();
            this.DataContext = new PatientCardViewModel();
        }

        public MainInfoPage(int patientId)
        {
            InitializeComponent();
            this.DataContext = new PatientCardViewModel(patientId);

        }
    }
}
