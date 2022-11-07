using Dental.Models;
using Dental.ViewModels.Materials;
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

namespace Dental.Views.NomenclatureDir
{
    /// <summary>
    /// Логика взаимодействия для NomenclatureControl.xaml
    /// </summary>
    public partial class NomenclatureControl : UserControl
    {
        public NomenclatureControl()
        {
            InitializeComponent();
            var db = new ApplicationContext();
            DataContext = new MaterialViewModel(db, db?.Nomenclature);
        }
    }
}
