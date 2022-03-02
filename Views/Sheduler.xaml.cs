using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduling;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;

namespace Dental.Views
{
    public partial class Sheduler : Page
    {
        public Sheduler()
        {
            InitializeComponent();
        }

        void OnAppointmentWindowShowing(object sender, AppointmentWindowShowingEventArgs e)
        {
            var vm = (ShedulerViewModel)((FrameworkElement)e.Source).DataContext;
            e.Window.DataContext = ClientAppointmentWindowViewModel.Create(
                e.Appointment,
                this.scheduler,
                vm.Clients,
                vm.ClassificatorCategories,
                vm.LocationAppointments
                );
        }
        void OnDropAppointment(object sender, DropAppointmentEventArgs e)
        {
            e.Cancel = e.ConflictedAppointments.Where(x => x.Count > 0).FirstOrDefault() != null;
        }

        void OnStartAppointmentDragFromOutside(object sender, StartAppointmentDragFromOutsideEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IEnumerable<Client>)))
                ((IEnumerable<Client>)e.Data.GetData(typeof(IEnumerable<Client>))).ToList().ForEach(x => e.DragAppointments.Add(CreateAppointment(x)));
        }

        void OnStartRecordDrag(object sender, StartRecordDragEventArgs e)
        {
            e.Data.SetData(typeof(IEnumerable<Client>), e.Records.Cast<Client>());
            e.Handled = true;
        }
        
        AppointmentItem CreateAppointment(Client patient)
        {
            AppointmentItem result = new AppointmentItem();
            result.CustomFields["PatientId"] = patient.Id;
            result.Subject = patient.FullName;
            result.StatusId = 2; ////////////////
            result.Start = DateTime.Today;
            result.End = result.Start.AddMinutes(20);
            return result;
        }

        void OnCompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        void OnDragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        void OnDropRecord(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
        }
    }
}
