using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Dental.Models
{
    [Table("PriceForClients")]
    class PriceForClients : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public int? ClassificatorId { get; set; }
        public Classificator Classificator { get; set; }

        public int? PriceRateForClientsId
        {
            get => _PriceRateForClientsId;
            set
            {
                _PriceRateForClientsId = value;
                OnPropertyChanged(nameof(_PriceRateForClientsId));
            }
        }
        private int? _PriceRateForClientsId;




        [Required(ErrorMessage = @"Поле ""Название тарифа"" обязательно для заполнения")]
        public PriceRateForClients PriceRateForClients 
        {
            get => _PriceRateForClients; 
            set  
            {
                _PriceRateForClients = value;
                OnPropertyChanged(nameof(PriceRateForClients));
            } 
        }
        private PriceRateForClients _PriceRateForClients;

        public string Price { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
