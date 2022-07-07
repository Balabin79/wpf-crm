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
    public class FieldsViewModel
    {
        private readonly ApplicationContext db;

        public FieldsViewModel(Client client, ApplicationContext db)
        {
            try
            {
                this.db = db;
                AdditionalFieldsVisible = Visibility.Collapsed;

                // получаем все поля для раздела
                AdditionalFields = db.AdditionalClientFields.ToArray();
                if (AdditionalFields.Count() == 0) return;

                Fields = new ObservableCollection<LayoutItem>();

                // загружаем значения полей
                AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == client.Id).ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>();

                ClientFieldsLoading();
            }
            catch (Exception e)
            {

            }
        }

        public FieldsViewModel(Employee employee, ApplicationContext db)
        {
            this.db = db;
        }


        private void ClientFieldsLoading()
        {

            foreach (var field in AdditionalFields)
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
        
        public ICollection<AdditionalClientField> AdditionalFields { get; set; }

        public ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }
        public ICollection<AdditionalEmployeeValue> AdditionalEmployeeValues { get; set; }

        public Visibility AdditionalFieldsVisible { get; set; } = Visibility.Hidden;     
    }
}
