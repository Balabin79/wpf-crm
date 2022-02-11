using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class UserActionsViewModel : ViewModelBase
    {
        
        public UserActionsViewModel()
        {
            try
            {
                ExpandCommand = new LambdaCommand(OnExpandCommandExecuted, CanExpandCommandExecute);

                db = new ApplicationContext();
                Collection = db.UserActions.OrderBy(f => f.CreatedAt).ToArray();
            } catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand ExpandCommand { get; }
        private bool CanExpandCommandExecute(object p) => true;
        private void OnExpandCommandExecuted(object p) 
        {
            if (p is GridControl grid) { 
                bool expanded = false;
                for (int i = 1; i < Collection.Count; i++)
                {
                    if (grid.IsGroupRowExpanded(i)) expanded = true;
                }
                if (expanded) grid.CollapseAllGroups(); 
                else grid.ExpandAllGroups();
            }           
        }

        public ICollection<UserActions> Collection { get; }
        ApplicationContext db;
    }
}

