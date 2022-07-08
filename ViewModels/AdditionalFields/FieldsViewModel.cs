using Dental.Models;
using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;

namespace Dental.ViewModels.AdditionalFields
{
    public class FieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        //public delegate void ChangeVisibleTab(Visibility visibility);
        //public event ChangeVisibleTab EventChangeVisibleTab;
        public FieldsViewModel(Client client, PatientListViewModel vm)
        {
            try
            {
                this.db = vm.db;
               // AdditionalFieldsVisible = Visibility.Hidden;
            
                // получаем все поля для раздела
                AdditionalClientFields = db.AdditionalClientFields.ToArray();
                if (AdditionalClientFields.Count() == 0) return;

                Fields = new ObservableCollection<LayoutItem>();

                // загружаем значения полей
                AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == client.Id).ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>();

                ClientFieldsLoading();

                if (Fields.Count > 0) AdditionalFieldsVisible = Visibility.Visible;
            }
            catch (Exception e)
            {

            }
        }

        public FieldsViewModel(Employee employee, ListEmployeesViewModel vm)
        {
            this.db = vm.db;
           // AdditionalFieldsVisible = Visibility.Hidden;
        
        // получаем все поля для раздела
        AdditionalEmployeeFields = db.AdditionalEmployeeFields.ToArray();
            if (AdditionalEmployeeFields.Count() == 0) return;

            Fields = new ObservableCollection<LayoutItem>();

            // загружаем значения полей
            AdditionalEmployeeValues = db.AdditionalEmployeeValue.Where(f => f.EmployeeId == employee.Id).ToObservableCollection() ?? new ObservableCollection<AdditionalEmployeeValue>();

            EmployeeFieldsLoading();

            if (Fields.Count > 0) AdditionalFieldsVisible = Visibility.Visible;
        }

        private void EmployeeFieldsLoading()
        {

            foreach (var field in AdditionalEmployeeFields)
            {

                var label = new LayoutItem()
                {
                    Label = field.Label,
                    LabelPosition = LayoutItemLabelPosition.Top,
                    Content = new TextEdit(),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                //var f = new TextEdit() { }
                Fields.Add(label);
            }

            /*

            foreach (var val in values)
            {

                var field = fields.FirstOrDefault(f => f.Id == val.AdditionalFieldId);
                if (field == null) continue;

                // получаем ссылку на шаблон этого поля и записываем его в TemplateField
            }*/

        }

        private void ClientFieldsLoading()
        {

            foreach (var field in AdditionalClientFields)
            {

                var label = new LayoutItem()
                {
                    Label = field.Label,
                    LabelPosition = LayoutItemLabelPosition.Top,
                    Content = new TextEdit(), 
                    Margin = new Thickness(0, 0, 0, 5)
                };
                //var f = new TextEdit() { }
                Fields.Add(label);
            }

            /*

            foreach (var val in values)
            {

                var field = fields.FirstOrDefault(f => f.Id == val.AdditionalFieldId);
                if (field == null) continue;

                // получаем ссылку на шаблон этого поля и записываем его в TemplateField
            }*/

        }


        public ICollection<LayoutItem> Fields { get; set; }
        
        public ICollection<AdditionalClientField> AdditionalClientFields { get; set; }
        public ICollection<AdditionalEmployeeField> AdditionalEmployeeFields { get; set; }

        public ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }
        public ICollection<AdditionalEmployeeValue> AdditionalEmployeeValues { get; set; }

        public Visibility AdditionalFieldsVisible { get; set; } = Visibility.Hidden;
    }
}
