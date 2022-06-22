﻿using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Services;
using System.Collections.Generic;
using Dental.Infrastructures.Logs;
using Dental.Views.EmployeeDir;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;
using Dental.Services.Files;

namespace Dental.ViewModels
{
    public class ListEmployeesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public ListEmployeesViewModel()
        {
            try
            {              
                db = new ApplicationContext();
                Collection = db.Employes.OrderBy(d => d.LastName).Include("Sex").ToObservableCollection();
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
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список сотрудников\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenFormEmployeeCard(object p)
        {
            try
            {
                EmployeeWin = (p != null) 
                    ? new EmployeeCardWindow(Collection.Where(f => f.Id == (int)p).FirstOrDefault(), this)
                    : new EmployeeCardWindow(new Employee(), this);
                EmployeeWin.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта сотрудника\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ExpandAll(object p)
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

        [Command]
        public void NavigateTo(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(p.ToString())) return;

                if (!int.TryParse(p.ToString(), out int param)) param = 0;                
                if(Application.Current.Resources["Router"] is Navigator nav)
                {
                    if (param == -1 || param == 0) nav.LeftMenuClick("Dental.Views.EmployeeDir.Employee");
                    else nav.LeftMenuClick(new object[] { "Dental.Views.EmployeeDir.Employee", param });
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                DocumentsWindow = new DocumentsWindow() { DataContext = new EmployeesDocumentsViewModel() };
                DocumentsWindow.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ObservableCollection<Employee> Collection { get; set; }
   
        public EmployeeCardWindow EmployeeWin { get; set; }
        public DocumentsWindow DocumentsWindow { get; set; }
    }
}
