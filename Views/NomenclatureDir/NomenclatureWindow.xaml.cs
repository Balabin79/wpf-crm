using Dental.Models;
using System.Collections.Generic;
using System.Windows;

namespace Dental.Views.NomenclatureDir
{
    public partial class NomenclatureWindow : Window
    {
        public NomenclatureWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MeasuresProp = DependencyProperty.Register(
            "Measures",
            typeof(ICollection<Measure>),
            typeof(NomenclatureWindow)
        );

        public ICollection<Measure> Measures
        {
            get => (ICollection<Measure>)GetValue(MeasuresProp);
            set => SetValue(MeasuresProp, value);
        }

        private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (e.NewValue?.ToString() == "0")
            {
                ((DevExpress.Xpf.Editors.BaseEdit)sender).EditValue = "";
            }
            e.Handled = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

    }
}
