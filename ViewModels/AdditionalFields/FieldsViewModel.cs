﻿using Dental.Models;
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
using System.Data.Entity;
using System.Globalization;
using Dental.Infrastructures.Converters;
using System.Windows.Data;
using Dental.ViewModels.EmployeeDir;
using Dental.ViewModels.ClientDir;

namespace Dental.ViewModels.AdditionalFields
{
    public class FieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public FieldsViewModel(Client client)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    // получаем все поля для раздела
                    AdditionalClientFields = db.AdditionalClientFields.Include(f => f.TypeValue).ToArray();
                    Fields = new ObservableCollection<LayoutItem>();
                    if (AdditionalClientFields.Count() == 0) return;

                    // загружаем значения полей
                    AdditionalClientValues = (client != null) ? db.AdditionalClientValue?.Where(f => f.ClientId == client.Id)?.ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>() : new ObservableCollection<AdditionalClientValue>();
                }       

                ClientFieldsLoading(client);

                if (Fields.Count > 0) AdditionalFieldsVisible = Visibility.Visible;           
            }
            catch (Exception e)
            {

            }
        }

        private void ClientFieldsLoading(Client client)
        {            
            foreach (var i in AdditionalClientFields)
            {
                var value = AdditionalClientValues.FirstOrDefault(f => f.AdditionalFieldId == i.Id && f.ClientId == client.Id);
                var binding = new Binding() 
                { 
                    Source = this, 
                    Path = new PropertyPath("IsReadOnly"), 
                    Mode = BindingMode.TwoWay, 
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
                };

                var el = GetField(i?.TypeValue?.SysName, value?.Value);
                el.SetBinding(BaseEdit.IsReadOnlyProperty, binding);

                var label = new LayoutItem()
                {
                    Label = i.Label,
                    LabelPosition = LayoutItemLabelPosition.Top,
                    Content = el,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Fields.Add(label);
            }
        }

        private BaseEdit GetField(string sysName, string value)
        {
            try
            {             
                switch (sysName)
                {
                    case "string":
                        return new TextEdit() { EditValue = value };
                    case "money": return new TextEdit() { Mask = "c", MaskType = MaskType.Numeric, MaskCulture = CultureInfo.CurrentCulture, DisplayFormatString = "{}{0:c2}", EditValue = value };
                    case "int": return new TextEdit() { Mask = "d", MaskType = MaskType.Numeric, EditValue = value };

                    case "float": return new TextEdit() { Mask = "f", MaskType = MaskType.Numeric, EditValue = value };
                    case "date": return new DateEdit() { EditValue = value, Text = value,
                        Mask = "d",
                        MaskCulture = CultureInfo.CurrentCulture,
                        DisplayTextConverter = new DateToStringConverter()
                    };
                    case "datetime": return new DateEdit() { DisplayTextConverter = new DateToStringConverter(), EditValue = value, Mask = "G", StyleSettings = new DateEditNavigatorWithTimePickerStyleSettings(), MaskCulture = CultureInfo.CurrentCulture, Text = value };
                    case "percent": return new TextEdit() { Mask = "p", MaskType = MaskType.Numeric, EditValue = value };
                    default: return new TextEdit() { EditValue = value };
                }
            }
            catch(Exception e)
            {
                return new TextEdit() { EditValue = value };
            }

        }

        public bool Save(Client client)
        {
            try
            {   using (var db = new ApplicationContext())
                {
                    foreach (var i in Fields)
                    {
                        var value = db.AdditionalClientValue.FirstOrDefault(f => f.AdditionalField.Label.ToString() == i.Label.ToString() && f.ClientId == client.Id);
                        var val = ((BaseEdit)i.Content).EditValue?.ToString();
                        if (value == null && val != null)
                        {
                            var item = new AdditionalClientValue() { ClientId = client.Id, Value = val, AdditionalFieldId = AdditionalClientFields.FirstOrDefault(f => f.Label.ToString() == i.Label.ToString()).Id };
                            db.AdditionalClientValue.Add(item);
                        }
                        if (value != null) value.Value = val;
                    }
                    return db.SaveChanges() > 0;
                }                    
            }
            catch
            {
                return false;
            }

        }


        public ICollection<LayoutItem> Fields { get; set; }
        
        public ICollection<AdditionalClientField> AdditionalClientFields { get; set; }

        public ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }

        public Visibility AdditionalFieldsVisible { get; set; } = Visibility.Hidden;

        public void ChangedReadOnly(bool status)
        {
            IsReadOnly = status;
        }
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

    }
}
