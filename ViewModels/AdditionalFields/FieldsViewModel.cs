using B6CRM.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using B6CRM.Infrastructures.Converters;
using System.Windows.Data;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.ViewModels.ClientDir;

namespace B6CRM.ViewModels.AdditionalFields
{
    public class FieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public FieldsViewModel(Client client)
        {
            try
            {
                db = new ApplicationContext();
                ClientFieldsLoading(client);
                if (Fields.Count > 0) AdditionalFieldsVisible = Visibility.Visible;

                if (Application.Current.Resources["ClientCardDispatcher"] is ClientCardDispatcher dispatcher) 
                {
                    IsReadOnly = client?.Id == 0 ? true : dispatcher.IsReadOnly;
                } 
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public void ClientFieldsLoading(Client client)
        {
            Fields = new ObservableCollection<LayoutItem>();
            var additionalClientFields = db.AdditionalClientFields.Include(f => f.TypeValue).OrderBy(f => f.Sort).ToArray();

            if (additionalClientFields.Count() == 0) return;

            // загружаем значения полей
            var AdditionalClientValues = client != null ? db.AdditionalClientValue?.Where(f => f.ClientId == client.Id)?.ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>() : new ObservableCollection<AdditionalClientValue>();

            foreach (var i in additionalClientFields)
            {
                var model = AdditionalClientValues.FirstOrDefault(f => f.AdditionalFieldId == i.Id && f.ClientId == client.Id);
                var binding = new Binding()
                {                  
                    Source = this,
                    Path = new PropertyPath("IsReadOnly"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                var el = GetField(i?.TypeValue?.SysName, i?.SysName, model);
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

        private BaseEdit GetField(string sysType, string sysName, AdditionalClientValue model)
        {
            try
            {
                switch (sysType)
                {
                    case "string":
                        return new TextEdit() { EditValue = model?.Value, Name = sysName };

                    case "money":
                        return new TextEdit()
                        {
                            Name = model?.AdditionalField?.SysName,
                            Mask = "c",
                            MaskType = MaskType.Numeric,
                            MaskCulture = CultureInfo.CurrentCulture,
                            MaskUseAsDisplayFormat = true,
                            DisplayFormatString = "{}{0:c2}",
                            EditValue = decimal.TryParse(model?.Value, out decimal val) ? string.Format("{0:c2}", val) : model?.Value
                        };

                    case "int":
                        return new TextEdit() { Mask = "d", MaskType = MaskType.Numeric, EditValue = model?.Value, Name = sysName };

                    case "float":
                        return new TextEdit() { Mask = "f", MaskType = MaskType.Numeric, EditValue = model?.Value, Name = sysName };

                    case "date":
                        var dt = DateTime.TryParse(model?.Value, out DateTime dateTime) ? dateTime.ToShortDateString() : model?.Value;
                        return new DateEdit()
                        {
                            Name = sysName,
                            EditValue = dt,
                            Text = dt,
                            Mask = "d",
                            MaskCulture = CultureInfo.CurrentCulture,
                            MaskType = MaskType.DateTime
                        };

                    case "datetime": return new DateEdit() { DisplayTextConverter = new DateToStringConverter(), EditValue = model?.Value, Name = sysName, Mask = "G", StyleSettings = new DateEditNavigatorWithTimePickerStyleSettings(), MaskCulture = CultureInfo.CurrentCulture, Text = model?.Value };
                    case "percent": return new TextEdit() { Mask = "p", MaskType = MaskType.Numeric, EditValue = model?.Value , Name = sysName };
                    default: return new TextEdit() { EditValue = model?.Value, Name = sysName };
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
                return new TextEdit() { EditValue = model?.Value, Name = sysName };
            }

        }

        public void Save(Client client)
        {
            try
            {
                foreach (var i in Fields)
                {
                    if (i.Content is BaseEdit el)
                    {
                        var val = el.EditValue?.ToString();
                        var sysName = el.Name?.ToString();


                        var value = db.AdditionalClientValue.Include(f => f.AdditionalField).Where(
                            f => f.AdditionalField.SysName == sysName && f.ClientId == client.Id).FirstOrDefault();

                        if (value == null && val != null)
                        {
                            var item = new AdditionalClientValue() 
                            {
                                ClientId = client.Id, 
                                Value = val, 
                                AdditionalFieldId = db.AdditionalClientFields.FirstOrDefault(f => f.SysName == sysName).Id 
                            };
                            db.AdditionalClientValue.Add(item);
                        }
                        if (value != null) value.Value = val;
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public ObservableCollection<LayoutItem> Fields
        {
            get { return GetProperty(() => Fields); }
            set { SetProperty(() => Fields, value); }
        }

        public Visibility AdditionalFieldsVisible { get; set; } = Visibility.Collapsed;

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
