using DevExpress.Xpf.Core;
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
using System.Windows.Shapes;

namespace Dental.Views.WindowForms
{
    /// <summary>
    /// Логика взаимодействия для IDSWindow.xaml
    /// </summary>
    public partial class IDSWindow : Window
    {
        public IDSWindow()
        {
            InitializeComponent();
        }

        private void RichEdit_DocumentClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RichEdit.Modified == true)
            {
              var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения. Если не хотите их потерять, нажмите \"Нет\", а затем сохраните документ.", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No")
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = false;
            }
        }
    }
}
