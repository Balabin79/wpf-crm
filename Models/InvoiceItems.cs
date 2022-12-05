using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("InvoiceItems")]
    public class InvoiceItems : AbstractBaseModel
    {
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value); }
        }

        public int Type
        {
            get { return GetProperty(() => Type); }
            set { SetProperty(() => Type, value); }
        }

        public IInvoiceItem Item
        {
            get { return GetProperty(() => Item); }
            set { SetProperty(() => Item, value); }
        }

        public int? ItemId
        {
            get { return GetProperty(() => ItemId); }
            set { SetProperty(() => ItemId, value); }
        }

        public Invoice Invoice { get; set; }
        public int? InvoiceId { get; set; }       

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

        public override string ToString() => Name + " (" + Code + ")";
    }
}
