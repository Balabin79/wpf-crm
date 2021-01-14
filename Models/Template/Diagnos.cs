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

                if (model.Id == model.ParentId && model.Dir == 0)
                {
                    // это корневой файл
                    // показываем форму с выбором корневой и обычной папки и файла
                    int x = 0;
                }
                if (model.Id == model.ParentId && model.Dir == 1)
                {
                    // это корневая папка 
                    // показываем форму с выбором корневой и обычной папки и файла
                    int x = 0;
                }

                else if (model.Dir == 1 && model.ParentId != model.Id)
                {
                    // это вложенная папка, 
                    // показываем форму с выбором файла и обычной папки, присваиваем записи ParentId = текущему Id
                    int x = 0;
                }


                if (model.Dir == 0 && model.ParentId != 0)
                {
                    // это файл в папке
                    // показываем форму с выбором файла и обычной папки, присваиваем записи ParentId = текущему ParentId
                    int x = 0;
                }

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





    

