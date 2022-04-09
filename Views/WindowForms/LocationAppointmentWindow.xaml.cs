using Dental.Models;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Dental.Views.WindowForms
{
    public partial class LocationAppointmentWindow : Window
    {
        public LocationAppointmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                
                if (((ViewModels.ShedulerViewModel)((FrameworkElement)sender).DataContext).LocationAppointments is ObservableCollection<LocationAppointment> collection)
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
                    var forRemoving = new ConcurrentBag<LocationAppointment>();
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
