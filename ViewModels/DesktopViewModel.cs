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
    class DesctopViewModel : ViewModelBase
    {
        
        public DesctopViewModel()
        {
            try
            {
                db = new ApplicationContext();
               
            } catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Рабочий стол\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
     

        ApplicationContext db;
    }
}

