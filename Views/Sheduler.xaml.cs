using Dental.Models;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduling;
using System.Windows.Controls;

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

            e.Window.DataContext = ClientAppointmentWindowViewModel.Create(e.Appointment, this.scheduler, ((ShedulerViewModel)((System.Windows.FrameworkElement)e.Source).DataContext).Clients);
            //e.Window.DataContext = ((System.Windows.FrameworkElement)e.Source).DataContext;
        }
        void OnDropAppointment(object sender, DropAppointmentEventArgs e)
        {
           // e.Cancel = e.ConflictedAppointments.Where(x => x.Count > 0).FirstOrDefault() != null;
        }
        void OnStartAppointmentDragFromOutside(object sender, StartAppointmentDragFromOutsideEventArgs e)
        {
           /* if (e.Data.GetDataPresent(typeof(IEnumerable<Patient>)))
                ((IEnumerable<Patient>)e.Data.GetData(typeof(IEnumerable<Patient>))).ToList().ForEach(x => e.DragAppointments.Add(CreateAppointment(x)));*/
        }
        void OnStartRecordDrag(object sender, StartRecordDragEventArgs e)
        {
           // e.Data.SetData(typeof(IEnumerable<Patient>), e.Records.Cast<Patient>());
           // e.Handled = true;
        }
        
        AppointmentItem CreateAppointment(PatientInfo patient)
        {
            AppointmentItem result = new AppointmentItem();
            //result.CustomFields["PatientId"] = patient.Id;
            //result.Subject = patient.FullName;
           // result.StatusId = ReceptionDeskData.PaymentStateNotYetBilled.Id;
            //result.Start = DateTime.Today;
            result.End = result.Start.AddMinutes(20);
            return result;
        }

        void OnCompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }
        void OnDragRecordOver(object sender, DragRecordOverEventArgs e)
        {
           // e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
        void OnDropRecord(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
        }

        private void ListBoxEdit_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int x = 0;
        }
    }
}
