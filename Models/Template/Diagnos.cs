using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using System;
using Dental.Interfaces;
using Dental.Infrastructures.Collection;
using Dental.Models.Base;
using Dental.Repositories.Template;
using System.Collections.ObjectModel;

namespace Dental.Models.Template
{
    [Table("Diagnoses")]
    class Diagnos : TreeModelBase, ITreeViewCollection
    {
     
        public Diagnos()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
        }


        public  ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                 Diagnos model = (Diagnos)p;
                 if (new DeleteInTree().run(model.Dir)) DiagnosRepository.Delete(model);

            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }

        private void OnAddCommandExecuted(object p)
        {
            try
            {
                Diagnos model = (Diagnos)p;

                

            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }
        }
        [NotMapped]
        public static ObservableCollection<Diagnos> Collection { get; set; } = DiagnosRepository.Collection;

    }
}





    

