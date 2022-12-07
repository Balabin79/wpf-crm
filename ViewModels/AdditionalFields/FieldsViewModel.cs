using Dental.Models;
using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using System.Data.Entity;
using System.Globalization;
using Dental.Infrastructures.Converters;
using System.Windows.Data;

namespace Dental.ViewModels.AdditionalFields
{
    public class FieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public FieldsViewModel(Client client, ApplicationContext ctx)
        {
            try
            {
                db = ctx;
                ClientFieldsLoading(client);
                if (Fields.Count > 0) AdditionalFieldsVisible = Visibility.Visible;
            }
            catch {}
        }

        public void ClientFieldsLoading(Client client)
        {
            Fields = new ObservableCollection<LayoutItem>();
            var AdditionalClientFields = db.AdditionalClientFields.Include(f => f.TypeValue).ToArray();
            if (AdditionalClientFields.Count() == 0) return;

            // загружаем значения полей
            var AdditionalClientValues = (client != null) ? db.AdditionalClientValue?.Where(f => f.ClientId == client.Id)?.ToObservableCollection() ?? new ObservableCollection<AdditionalClientValue>() : new ObservableCollection<AdditionalClientValue>();

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
                    case "date":
                        return new DateEdit()
                        {
                            EditValue = value,
                            Text = value,
                            Mask = "d",
                            MaskCulture = CultureInfo.CurrentCulture,
                            DisplayTextConverter = new DateToStringConverter()
                        };
                    case "datetime": return new DateEdit() { DisplayTextConverter = new DateToStringConverter(), EditValue = value, Mask = "G", StyleSettings = new DateEditNavigatorWithTimePickerStyleSettings(), MaskCulture = CultureInfo.CurrentCulture, Text = value };
                    case "percent": return new TextEdit() { Mask = "p", MaskType = MaskType.Numeric, EditValue = value };
                    default: return new TextEdit() { EditValue = value };
                }
            }
            catch (Exception e)
            {
                return new TextEdit() { EditValue = value };
            }

        }

        public void Save(Client client)
        {
            try
            {
                foreach (var i in Fields)
                {
                    var value = db.AdditionalClientValue.FirstOrDefault(f => f.AdditionalField.Label.ToString() == i.Label.ToString() && f.ClientId == client.Id);
                    var val = ((BaseEdit)i.Content).EditValue?.ToString();
                    if (value == null && val != null)
                    {
                        var item = new AdditionalClientValue() { ClientId = client.Id, Value = val, AdditionalFieldId = db.AdditionalClientFields.FirstOrDefault(f => f.Label.ToString() == i.Label.ToString()).Id };
                        db.AdditionalClientValue.Add(item);
                    }
                    if (value != null) value.Value = val;
                }
            }
            catch{}
        }

        public ObservableCollection<LayoutItem> Fields
        {
            get { return GetProperty(() => Fields); }
            set { SetProperty(() => Fields, value); }
        }

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
