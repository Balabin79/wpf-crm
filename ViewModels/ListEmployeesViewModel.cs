using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Services;
using System.Collections.Generic;
using Dental.Infrastructures.Logs;

namespace Dental.ViewModels
{
    class ListEmployeesViewModel : ViewModelBase
    {

        private ApplicationContext db;
        public ListEmployeesViewModel()
        {
            try
            {
                NavigateToCommand = new LambdaCommand(OnNavigateToCommandExecuted, CanNavigateToCommandExecute);
                db = new ApplicationContext();
                Collection = db.Employes.OrderBy(d => d.LastName).Include(f => f.Status).Include(f => f.Sex).Include(f => f.EmployesSpecialities.Select(i => i.Speciality)).ToList();
                foreach (var i in Collection)
                {
                    if (!string.IsNullOrEmpty(i.Photo) && File.Exists(i.Photo))
                    {
                        using (var stream = new FileStream(i.Photo, FileMode.Open))
                        {
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.StreamSource = stream;
                            img.EndInit();
                            img.Freeze();
                            i.Image = img;
                        }
                        //i.Image = new BitmapImage(new Uri(i.Photo));
                    }
                    else i.Image = null;

                    
                }
            }      
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand NavigateToCommand { get; }
        private bool CanNavigateToCommandExecute(object p) => true;
        private void OnNavigateToCommandExecuted(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(p.ToString())) return;
                var nav = Navigation.Instance;
                int.TryParse(p.ToString(), out int param);
                if (param == -1 || param == 0) nav.LeftMenuClick.Execute("Dental.Views.Employee");
                else nav.LeftMenuClick.Execute(new object[] { "Dental.Views.Employee", param });
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public List<Employee> Collection { get; set; }
    }
}
