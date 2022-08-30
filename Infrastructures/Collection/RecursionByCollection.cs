using Dental.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;

namespace Dental.Infrastructures.Collection
{
    class RecursionByCollection
    {
        public RecursionByCollection(ObservableCollection<ITreeModel> collection, ITreeModel model)
        {
            Collection = collection;
            Model = model;
        }

        public ObservableCollection<ITreeModel> GetItemChilds()
        {
            FindItemChilds(Collection, Model);
            return Collection.Where(f => Child.Contains(f.Id)).Distinct().ToObservableCollection();
        }


        public ObservableCollection<ITreeModel> GetDirectories()
        {
            IsOnlyFindDir = true;
            FindItemChilds(Collection, Model);
            return Collection.Where(f => f.IsDir == 1 && !Child.Contains(f.Id)).ToObservableCollection();
        }

        private void FindItemChilds(ObservableCollection<ITreeModel> collection, ITreeModel model)
        {
            var childs = collection.Where(f => f.ParentId == model.Id).ToList();

            foreach (var item in childs)
            {
                if (item.IsDir == 1 && IsOnlyFindDir) FindItemChilds(collection, item);
                else
                {
                    Child.Add(model.Id);
                    FindItemChilds(collection, item);
                }
            }
            Child.Add(model.Id);
        }

        private List<int> Child { get; set; } = new List<int>();
        private ObservableCollection<ITreeModel> Collection { get; set; }
        private ITreeModel Model { get; set; }
        private bool IsOnlyFindDir {get; set;} = false;
 
    }
}
