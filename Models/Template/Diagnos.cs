using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Dental.Repositories.Template;
using Dental.Infrastructures.Commands.Base;
using DevExpress.Xpf.Core;
using System;
using Dental.Interfaces;
using Dental.Infrastructures.Collection;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Dental.Models.Base;

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

        
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                Diagnos model = (Diagnos)p;
                if (new DeleteInTree().run(model.Dir)) Delete(model);

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
                if (model.Id == model.ParentId && model.Dir == 1 )
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
        public static ObservableCollection<Diagnos> Collection{ get; set; } = GetDiagnoses();


        public int Delete(Diagnos diagnos)
        {
            Diagnos model = (Diagnos)diagnos;
            using (ApplicationContext db = new ApplicationContext())
            {
                var list = Recursion(model, new List<Diagnos>() { model });

                list.ToList().ForEach(d => list.ToList().Remove(d));
                list.ToList().ForEach(d => Collection.Remove(d));
                return db.SaveChanges();
            }
        }

        public List<Diagnos> Recursion(Diagnos model, List<Diagnos> nodes)
        {  
            List<Diagnos> list = Collection.Where(d => d.ParentId == model.Id).ToList();
           
            if (list.Count>0)
            {
                foreach (Diagnos item in list)
                {
                    if (item.ParentId != item.Id && item.Dir == 1) Recursion(item, nodes);
                    nodes.Add(item);
                }
            }
            return nodes;
        }


        public static ObservableCollection<Diagnos> GetDiagnoses()
        {
            try
            {
                ApplicationContext db = new ApplicationContext();
                    db.Diagnoses.Load();
                    return db.Diagnoses.Local;
            }
            catch (Exception e)
            {
                return new ObservableCollection<Diagnos>();
            }

        }

    }
}
