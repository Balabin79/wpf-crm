using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Dental.Repositories.Template;
using Dental.Infrastructures.Commands.Base;
using DevExpress.Xpf.Core;
using System;
using System.Windows;
using System.Data.Entity;
using System.Linq;
using DevExpress.Data.Linq.Helpers;
using Dental.Repositories;
using Dental.Interfaces;
using Dental.Infrastructures.Collection;

namespace Dental.Models.Template
{
    [Table("Diagnoses")]
    class DiagnosRepository : ViewModelBase, ITreeViewCollection
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

        public IRepositoryCollection ClassRepository 
        {
            get => new Template.DiagnosRepository();
        }

        public DiagnosRepository()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
        }

        
        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                (new DeleteStrategy((ITreeViewCollection)p)).run();
            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        [NotMapped]
        public ObservableCollection<ITreeViewCollection> Diagnoses {
            get {
                if (_diagnoses == null) _diagnoses = DiagnosRepository.GetDiagnoses();
                return _diagnoses;
            }
            set {
                Set(ref _diagnoses, value); 
            }
        }
        private ObservableCollection<ITreeViewCollection>  _diagnoses;

    }
}
