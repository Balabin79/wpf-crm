using DevExpress.Xpf.Bars;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.PatientCard
{
    public partial class ToothControl : UserControl
    {
        
        public static readonly DependencyProperty AbbrProperty = DependencyProperty.Register
        (
             "Abbr",
             typeof(string),
             typeof(ToothControl),
             new PropertyMetadata(string.Empty)
        );

        public static readonly DependencyProperty ToothImagePathProperty = DependencyProperty.Register
        (
             "ToothImagePath",
             typeof(string),
             typeof(ToothControl),
             new PropertyMetadata(string.Empty)
        );


        public static readonly DependencyProperty ToothNumberProperty = DependencyProperty.Register
        (
             "ToothNumber",
             typeof(Dental.Models.Teeth),
             typeof(ToothControl),
             new PropertyMetadata(null)
        );


        public string Abbr
        {
            get { return (string)GetValue(AbbrProperty); }
            set { SetValue(AbbrProperty, value); }
        }

        public string ToothImagePath
        {
            get { return (string)GetValue(ToothImagePathProperty); }
            set { SetValue(ToothImagePathProperty, value); }
        }

        public Dental.Models.Teeth ToothNumber
        {
            get { return (Dental.Models.Teeth)GetValue(ToothNumberProperty); }
            set { SetValue(ToothNumberProperty, value); }
        }

        public ToothControl()
        {
            InitializeComponent();
            int x = 0;
        }

        public ToothControl(int toothNumber)
        {
            InitializeComponent();
            int x = 0;
        }

        public ToothControl(string toothNumber)
        {
            InitializeComponent();
            int x = 0;
        }
    }
}
