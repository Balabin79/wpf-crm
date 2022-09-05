using Dental.Models;
using System.Windows;
using System.Windows.Controls;

namespace Dental.Views.PatientCard
{
    public partial class ToothControl : UserControl
    {
        public static readonly DependencyProperty ToothProp = DependencyProperty.Register(
       "Tooth",
       typeof(Tooth),
       typeof(ToothControl)
       );

        public static readonly DependencyProperty RowPositionAbbrProp = DependencyProperty.Register(
            "RowPositionAbbr",
            typeof(string),
            typeof(ToothControl)
        );

        public static readonly DependencyProperty RowPositionNumberProp = DependencyProperty.Register(
            "RowPositionNumber",
            typeof(string),
            typeof(ToothControl)
        );

        public Tooth Tooth
        {
            get => (Tooth)GetValue(ToothProp);
            set => SetValue(ToothProp, value);
        }

        public string RowPositionAbbr
        {
            get => (string)GetValue(RowPositionAbbrProp);
            set => SetValue(RowPositionAbbrProp, value);
        }

        public string RowPositionNumber
        {
            get => (string)GetValue(RowPositionNumberProp);
            set => SetValue(RowPositionNumberProp, value);
        }

        public ToothControl()
        {
            InitializeComponent();
        }
    }
}
