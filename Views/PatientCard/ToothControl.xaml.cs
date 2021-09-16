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
             typeof(string),
             typeof(ToothControl),
             new PropertyMetadata(string.Empty)
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

        public string ToothNumber
        {
            get { return (string)GetValue(ToothNumberProperty); }
            set { SetValue(ToothNumberProperty, value); }
        }

        public ToothControl()
        {
            InitializeComponent();

        }
    }
}
