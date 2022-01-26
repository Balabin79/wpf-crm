using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.WindowsUI;
using System.Windows.Controls;

namespace Dental.Views.PatientCard
{
    public partial class MainInfoPage : Page, IUser
    {
        public MainInfoPage() : this(0) { }

        public MainInfoPage(int patientId)
        {
            InitializeComponent();
            this.DataContext = new PatientCardViewModel(patientId);
            UserId = patientId;
        }

        public int UserId { get; set; }
    }
}
