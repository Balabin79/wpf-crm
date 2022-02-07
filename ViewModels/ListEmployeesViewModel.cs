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
using Dental.Views.EmployeeDir;

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

                OpenFormSpecialitiesCommand = new LambdaCommand(OnOpenFormSpecialitiesExecuted, CanOpenFormSpecialitiesExecute);
                OpenFormCategoryEmployesCommand = new LambdaCommand(OnOpenFormCategoryEmployesExecuted, CanOpenFormCategoryEmployesExecute);               
                OpenFormNotificationSettingsCommand = new LambdaCommand(OnOpenFormNotificationSettingExecuted, CanOpenFormNotificationSettingExecute);

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
        public ICommand OpenFormSpecialitiesCommand { get; }
        public ICommand OpenFormCategoryEmployesCommand { get; }
        public ICommand OpenFormNotificationSettingsCommand { get; }

        private bool CanNavigateToCommandExecute(object p) => true;
        private bool CanExpandAllCommandExecute(object p) => true;
        private bool CanOpenFormSpecialitiesExecute(object p) => true;
        private bool CanOpenFormCategoryEmployesExecute(object p) => true;
        private bool CanOpenFormNotificationSettingExecute(object p) => true;


        private void OnOpenFormSpecialitiesExecuted(object p)
        {
            try
            {
                SpecialitiesWin = new SpecialitiesWindow();
                SpecialitiesWin.ShowDialog();
            }
            catch
            {

            }
        }

        private void OnOpenFormCategoryEmployesExecuted(object p)
        {
            try
            {
                GroupsWin = new EmployeeGroupsWindow();
                GroupsWin.ShowDialog();
            }
            catch
            {

            }
        }
        
        private void OnOpenFormNotificationSettingExecuted(object p)
        {
            try
            {
                NotificationSettingsWin = new NotificationSettingsWindow();
                NotificationSettingsWin.ShowDialog();
            }
            catch
            {

            }
        }

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

        private void OnNavigateToCommandExecuted(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(p.ToString())) return;

                if (!int.TryParse(p.ToString(), out int param)) param = 0;                
                if(Application.Current.Resources["Router"] is Navigator nav)
                {
                    if (param == -1 || param == 0) nav.LeftMenuClick.Execute("Dental.Views.EmployeeDir.Employee");
                    else nav.LeftMenuClick.Execute(new object[] { "Dental.Views.EmployeeDir.Employee", param });
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public List<Models.Employee> Collection { get; set; }

        public SpecialitiesWindow SpecialitiesWin { get; set; }
        public EmployeeGroupsWindow GroupsWin { get; set; }
        public NotificationSettingsWindow NotificationSettingsWin { get; set; }       
    }
}
