using Dental.Models;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.Pages.UserControls
{
    public partial class PatientControl : UserControl
    {
        public static readonly DependencyProperty IdProp = DependencyProperty.Register(
            "Id", 
            typeof(int), 
            typeof(PatientControl)
            );
        
        public static readonly DependencyProperty PageNameProp = DependencyProperty.Register(
            "PageName",
            typeof(string),
            typeof(PatientControl)
        );



        public int Id
        {
            get => (int)GetValue(IdProp);
            set => SetValue(IdProp, value);
        }

        public string PageName
        {
            get => (string)GetValue(PageNameProp);
            set => SetValue(PageNameProp, value);
        }


        public PatientControl()
        {
            InitializeComponent();
        }
    }
}
