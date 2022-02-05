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
                ExpandAllCommand = new LambdaCommand(OnExpandAllCommandExecuted, CanExpandAllCommandExecute);
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
                    }
                    else i.Image = null;
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand NavigateToCommand { get; }
        public ICommand ExpandAllCommand { get; }
        private bool CanNavigateToCommandExecute(object p) => true;
        private bool CanExpandAllCommandExecute(object p) => true;
        private void OnExpandAllCommandExecuted(object p)
        {
            try
            {
                if (p is DevExpress.Xpf.Grid.CardView card)
                {
                    if (card.IsCardExpanded(0)) card.CollapseAllCards(); 
                    else card.ExpandAllCards();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        } 

        public List<Employee> Collection { get; set; }

        private void OnNavigateToCommandExecuted(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(p.ToString())) return;

                int.TryParse(p.ToString(), out int param);
                if (Application.Current.Resources["Router"] is Navigator nav)
                {
                    if (param == -1 || param == 0) nav.LeftMenuClick.Execute("Dental.Views.Employee");
                    else nav.LeftMenuClick.Execute(new object[] { "Dental.Views.Employee", param });
                }                 
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

    }
}
