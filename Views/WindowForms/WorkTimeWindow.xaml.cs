using System;
using System.Windows;

namespace B6CRM.Views.WindowForms
{
    public partial class WorkTimeWindow : Window
    {
        public WorkTimeWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Form(object sender, RoutedEventArgs e) => Close();
    }
}
