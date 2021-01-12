using Dental.Interfaces;
using System;

namespace Dental.Infrastructures.Collection
{
    class DeleteStrategy
    {
        private ITreeViewCollection Model { get; set; }

        public DeleteStrategy(ITreeViewCollection model)
        {

            if (model.GetType().GetInterface("ICollection") != null) Model = model;
            else new Exception("Интерфейс не найден!");
        }

        public int run()
        {                       
            if (Model.Dir == 1) return (new DeleteDir()).Run(Model);
            return (new DeleteItem()).Run(Model);
        }
    }
}
