using System;
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
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.Invoices;
using Dental.Infrastructures.Converters;
using Dental.Views.NomenclatureDir;
using Nomenclature = Dental.Models.Nomenclature;
using Dental.ViewModels.ServicePrice;
using Dental.Views.ServicePrice;
using DevExpress.Xpf.Printing;
using System.Windows.Data;
using GroupInfo = DevExpress.Xpf.Printing.GroupInfo;
using Dental.Services;
using Dental.ViewModels.Base;

namespace Dental.ViewModels.Materials
{
    class MaterialViewModel : TreeBaseViewModel<Nomenclature>
    {
        private readonly ApplicationContext db;
        public MaterialViewModel(ApplicationContext ctx, DbSet<Nomenclature> context) : base(ctx, context)
        {
            try
            {
                db = ctx;
                Context = context;
                Collection = GetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Материалы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanSelectItemInServiceField(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanExpandTree(object p) => true;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureDeletable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanCancelForm() => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenByParentForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;
        public bool CanOpenDirByParentForm(object p) => ((UserSession)Application.Current.Resources["UserSession"]).NomenclatureEditable;

        public bool CanPrintPrice() => true;
        public bool CanLoadDocForPrint() => true;

        public override ObservableCollection<Nomenclature> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        protected override Window GetWindow() => new NomenclatureWindow();


        #region Печать
        public PrintServiceWindow PrintServiceWindow { get; set; }

        [Command]
        public void PrintPrice()
        {
            PrintServiceWindow = new PrintServiceWindow() { DataContext = this };
            PrintServiceWindow.Show();
        }

        [Command]
        public void LoadDocForPrint()
        {
            // Create a link and assign a data source to it.
            // Assign your data templates to different report areas.
            CollectionViewLink link = new CollectionViewLink();

            var db = new ApplicationContext();
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

        public ICollection<PrintService> SourceCollection { get; set; } = new List<PrintService>();

        private void SetSourceCollectttion()
        {
            db.Nomenclature?.GroupBy(f => f.Parent)?.Where(f => f.Key != null).ForEach(f => f.ForEach(
                i => SourceCollection?.Add(new PrintService() { ParentName = f.Key.Name, ServiceName = i.Name, Price = i.Price })));
        }
        #endregion

        protected override ObservableCollection<Nomenclature> GetCollection() => Context?.OrderBy(f => f.IsDir == 0).ThenBy(f => f.Name).Include(f => f.Parent).ToObservableCollection();
    }

}
