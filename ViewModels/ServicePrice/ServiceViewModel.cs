﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Views.WindowForms;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.ServicePrice;
using Dental.Infrastructures.Converters;
using DevExpress.Xpf.Printing;
using System.Windows.Data;
using GroupInfo = DevExpress.Xpf.Printing.GroupInfo;
using DevExpress.Mvvm;
using Dental.ViewModels.Base;
using Dental.Views.Settings;
using DevExpress.DataAccess.Sql;

namespace Dental.ViewModels.ServicePrice
{
    public class ServiceViewModel : TreeBaseViewModel<Service>
    {
        public ServiceViewModel(ApplicationContext ctx, DbSet<Service> context) : base(ctx, context)
        {
            try
            {
                db = ctx;
                Context = context;
                Collection = GetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Прайс услуг\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        protected override Window GetWindow() => new ServiceWindow();

        #region Печать
        private object Param { get; set; }

        [Command]
        public void PrintPrice(object p)
        {
            Param = p;
            PrintServiceWindow = new PrintServiceWindow() { DataContext = this };
            PrintServiceWindow.Show();
            
        }

        [Command]
        public void LoadDocForPrint()
        {            
            // Create a link and assign a data source to it.
            // Assign your data templates to different report areas.
            CollectionViewLink link = new CollectionViewLink();
            CollectionViewSource Source = new CollectionViewSource();

            if (int.TryParse(Param?.ToString(), out int result)) SetSourceCollectttion(result);         
            else SetSourceCollectttion();

            Source.Source = SourceCollection;

            Source.GroupDescriptions.Add(new PropertyGroupDescription("ParentName"));

            link.CollectionView = Source.View;
            link.GroupInfos.Add(new GroupInfo((DataTemplate)PrintServiceWindow.Resources["CategoryTemplate"]));
            link.DetailTemplate = (DataTemplate)PrintServiceWindow.Resources["ProductTemplate"];

            // Associate the link with the Document Preview control.
            PrintServiceWindow.preview.DocumentSource = link;

            // Generate the report document 
            // and show pages as soon as they are created.
            link.CreateDocument(true);
        }

        public ICollection<PrintService> SourceCollection { get; set; } = new List<PrintService>();

        private void SetSourceCollectttion(int? parentId = null)
        {
            SourceCollection = new List<PrintService>();

            var condition = parentId == null ? Context : Context?.Where(f => f.ParentID == parentId);

            condition?.GroupBy(f => f.Parent)?.Where(f => f.Key != null).
                ForEach(f => f.Where(d => d.IsDir != 1).
                ForEach(
                i => SourceCollection?.Add(new PrintService() { ParentName = f.Key.Name, ServiceName = i.Name, Price = i.Price })));
        }


        protected override ObservableCollection<Service> GetCollection() => Context?.OrderBy(f => f.IsDir == 0).ThenBy(f => f.Name).Include(f => f.Parent).ToObservableCollection();

        public PrintServiceWindow PrintServiceWindow { get; set; }
        #endregion
    }

    public class PrintService
    {
        public string ParentName { get; set; }
        public string ServiceName { get; set; }
        public decimal? Price { get; set; }
    }

}
