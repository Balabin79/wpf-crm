using Dental.Infrastructures.Attributes;
using Dental.Models.Base;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace Dental.Models
{
    [Table("Advertising")]
    public class Advertising : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название"" обязательно для заполнения")]
        public string Name { get; set; }


        public string DateFrom 
        {
            get { return GetProperty(() => DateFrom); }
            //set { SetProperty(() => DateFrom, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }

            set
            {
                if (DateTime.TryParse(value?.ToString(), out DateTime dateF) && DateTime.TryParse(DateTo?.ToString(), out DateTime dateT) && dateF > dateT)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Начало периода не может быть по времени позднее конца периода!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    value = null;
                }
                SetProperty(() => DateFrom, value);
            }
        }


        public string DateTo 
        {
            get { return GetProperty(() => DateTo); }
            set
            {
                if (DateTime.TryParse(value?.ToString(), out DateTime dateT) && DateTime.TryParse(DateFrom?.ToString(), out DateTime dateF) && dateT < dateF)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Конец периода не может быть по времени раньше начала периода!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    value = null;
                }
                SetProperty(() => DateTo, value);
            }
        }
        private string dateTo;
        

        public decimal? Cost { get; set; }

        public int? Active { get; set; } = 1;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
