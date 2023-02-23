using Dental.Models;
using Dental.Models.Templates;
using Dental.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dental.Views
{
    /// <summary>
    /// Логика взаимодействия для UserTemplateControl.xaml
    /// </summary>
    public partial class UserTemplateControl : UserControl
    {
        public UserTemplateControl()
        {
            InitializeComponent();
            var db = new ApplicationContext();
            DataContext = new TreeBaseViewModel<UserTemplate>(db, db?.UserTemplates);
        }
    }
}
