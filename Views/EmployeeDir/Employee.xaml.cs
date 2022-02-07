using Dental.ViewModels;
using System.Windows.Controls;

namespace Dental.Views.EmployeeDir
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Page, IUser
    {
        public Employee() : this(0){}

        public Employee(int id)
        {
            InitializeComponent();
            this.DataContext = new EmployeeViewModel(id);
            UserId = id;
        }

        public int UserId { get; set; }
    }
}
