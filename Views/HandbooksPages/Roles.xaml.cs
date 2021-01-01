using Dental.Models;
using Dental.Repositories;
using System.Windows.Controls;

namespace Dental.Views.HandbooksPages
{
    /// <summary>
    /// Логика взаимодействия для Roles.xaml
    /// </summary>
    public partial class Roles : Page
    {
        public Roles()
        {
            InitializeComponent();
        }

        private void TableView_RowUpdated(object sender, DevExpress.Xpf.Grid.RowEventArgs e)
        {

            var role = e.Row as Role;
            if (role != null && role.Id == 0)
            {
                RoleRepository.Add(role);  
            }
            else if (role != null && role.Id != 0)
            {
                RoleRepository.Update(role);
            }

        }


    }
}
