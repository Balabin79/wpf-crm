using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Models;
using Microsoft.EntityFrameworkCore;
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
using System.Threading.Tasks;

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
                IsReadOnly = Collection?.Count > 0;
            }
            catch(Exception e)
            {
                Log.ErrorHandler(e, "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Прайс услуг\"!", true);
            }
        }

        public bool CanPrintPrice() => ((UserSession)Application.Current.Resources["UserSession"]).PrintPrices;
        public bool CanLoadDocForPrint() => ((UserSession)Application.Current.Resources["UserSession"]).PrintPrices;


        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        #region Печать

        [Command]
        public void PrintPrice()
        {
            PrintServiceWindow = new PrintServiceWindow() { DataContext = this };
            PrintServiceWindow.Show();    
        }

        [Command]
        public void LoadDocForPrint()
        {
            try
            {
                // Create a link and assign a data source to it.
                // Assign your data templates to different report areas.
                CollectionViewLink link = new CollectionViewLink();
                CollectionViewSource Source = new CollectionViewSource();

                SetSourceCollectttion();

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
            catch(Exception e)
            {
                Log.ErrorHandler(e);
            }

        }

        public ICollection<PrintService> SourceCollection { get; set; } = new List<PrintService>();

        private void SetSourceCollectttion()
        {
            try
            {
                SourceCollection = new List<PrintService>();
                var markedItems = GetMarkedItems();

                var condition = markedItems?.Count > 0 ? Context?.Where(f => markedItems.Contains(f.ParentID)) : Context;

                condition?.ToList()
                    ?.GroupBy(f => f.Parent)?.Where(f => f.Key != null).ForEach(f => f.Where(d => d.IsDir != 1).
                    ForEach(
                    i => SourceCollection?.Add(new PrintService() { ParentName = f.Key.Name, ServiceName = i.Name, Price = i.Price })));
            }
            catch(Exception e)
            {
                Log.ErrorHandler(e);
            }
        }


        protected override ObservableCollection<Service> GetCollection() => Context?.OrderBy(f => f.IsDir == 0).ThenBy(f => f.Name).Include(f => f.Parent).ToObservableCollection();

        public PrintServiceWindow PrintServiceWindow { get; set; }

        private ICollection<int?> GetMarkedItems() => Collection?.Where(f => f.IsDir == 1 && f.Print)?.Select(f => f.Id)?.OfType<int?>().ToList();
        #endregion

        [AsyncCommand]
        public async Task CheckedIsHidden(object p)
        {
            try
            {
                if (p is Service model)
                {
                    await SetState(model, model.IsHidden);           
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке удаления произошла ошибка!", true);
            }
        }

        private async Task SetState(Service model, int? isHidden)
        {
            if (model?.IsDir == 1)
            {
                var nodes = Context.Where(f => f.ParentID == model.Id).ToArray();
                foreach (var node in nodes)
                {
                    if (node.IsDir == 1) 
                    {
                        node.IsHidden = isHidden;
                        await SetState(node, isHidden); 
                    }
                    node.IsHidden = isHidden;
                }
            }
        }

    }

    public class PrintService
    {
        public string ParentName { get; set; }
        public string ServiceName { get; set; }
        public decimal? Price { get; set; }
    }

}
