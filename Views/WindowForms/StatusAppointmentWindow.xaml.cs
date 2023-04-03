using B6CRM.Models;
using B6CRM.ViewModels;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace B6CRM.Views.WindowForms
{
    public partial class StatusAppointmentWindow : Window
    {
        public StatusAppointmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                
                if (((ShedulerViewModel)((FrameworkElement)sender).DataContext).StatusAppointments is ObservableCollection<AppointmentStatus> collection)
                {
                    bool isHaveUnsavedChange = false;
                    if (collection?.Count > 0)
                    {
                        // ищем несохраненные позиции
                        foreach (var item in collection)
                        {
                            if (item.Id == 0) isHaveUnsavedChange = true;
                        }
                    }

                    if (isHaveUnsavedChange)
                    {
                        string mes = "Имеются несохраненные позиции в списке! Закрыть форму?";
                        var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No")
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    // удаляем несохраненные позиции
                    // заводим коллекцию
                    var forRemoving = new ConcurrentBag<AppointmentStatus>();
                    foreach (var item in collection)
                    {
                        // складываем в нее элементы подлежащие удалению
                        if (item.Id == 0) forRemoving.Add(item);
                    }
                    // удаляем
                    foreach (var item in forRemoving) collection.Remove(item);

                }               
                e.Cancel = false;
            }
            catch
            {
                e.Cancel = false;
            }
        }
    }
}
