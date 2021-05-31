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
        public RecursionByCollection(ObservableCollection<ITreeModel> _Collection, ITreeModel _Model)
        {
            Collection = (ObservableCollection <ITreeModel>)_Collection;
            Model = _Model;
        }

        public ObservableCollection<ITreeModel> GetDirectories() 
        {
            FindDirectories(Collection, Model); 
            return Collection.Where(f => f.IsDir == 1 && !Child.Contains(f.Id)).ToObservableCollection();
        }

        
        public void FindDirectories(ObservableCollection<ITreeModel> collection, ITreeModel model)
        {
            var childs = collection.Where(f => f.ParentId == model.Id).ToList();

            foreach (var item in childs)
            {
                if (item.IsDir == 1) FindDirectories(collection, item);
            }
           
            Child.Add(model.Id);
        }

        private List<int> Child { get; set;} = new List<int>();
        private ObservableCollection<ITreeModel> Collection { get; set; }
        private ObservableCollection<ITreeModel> Result { get; set; } = new ObservableCollection<ITreeModel>();
        private ITreeModel Model { get; set; }
 
    }
}
