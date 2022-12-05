using Dental.Infrastructures.Converters;
using Dental.Models;
using Dental.Models.Base;
using Dental.Models.Templates;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace Dental.ViewModels.Invoices
{
    public class InvoiceItemVM : ViewModelBase, IDataErrorInfo
    {
        public delegate void SaveCommand(object m);
        public event SaveCommand EventSave;

        public InvoiceItemVM(int type, ApplicationContext db)
        {
            if (type == 0) Collection = db.Services.ToArray();
            else Collection = db.Nomenclature.ToArray();
            
        }

        public Invoice Invoice
        {
            get { return GetProperty(() => Invoice); }
            set { SetProperty(() => Invoice, value); }
        }

        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        public object SelectedItem
        {
            get { return GetProperty(() => SelectedItem); }
            set { SetProperty(() => SelectedItem, value); }
        }

        public InvoiceItems Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public IInvoiceItem Element
        {
            get { return GetProperty(() => Element); }
            set { SetProperty(() => Element, value); }
        }


        public int Type
        {
            get { return GetProperty(() => Type); }
            set { SetProperty(() => Type, value); }
        }

        public int Count
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public object[] Collection { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        [Command]
        public void SelectItemInField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.CurrentItem is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                    }
                    if (parameters.Tree.CurrentItem is Nomenclature item)
                    {
                        if (item.IsDir == 1) return;
                        parameters.Popup.EditValue = item;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

            }
        }

        [Command]
        public void Save(object p)
        {
            try
            {
                var item = Model.Id > 0 ? Invoice.InvoiceItems.FirstOrDefault(f => f.Id == Model.Id) : Model;

                if (SelectedItem is Service service)
                {
                    item.Count = Count;
                    item.Price = service?.Price;
                    item.Name = service?.Name;
                    item.Code = service?.Code;
                    item.ItemId = service?.Id;
                    item.Item = service;
                }

                if (SelectedItem is Nomenclature nom)
                {
                    Model.Count = Count;
                    Model.Type = Type;
                    Model.Price = nom?.Price;
                    Model.Name = nom?.Name;
                    Model.Code = nom?.Code;
                    item.ItemId = nom?.Id;
                    item.Item = nom;
                }

                EventSave?.Invoke(item);
                if (p is Window win) win?.Close();
            }
            catch
            {
                if (p is Window win) win?.Close();
            }
        }
    }
}
