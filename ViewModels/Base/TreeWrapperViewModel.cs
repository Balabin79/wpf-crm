using Dental.Infrastructures.Attributes;
using Dental.Infrastructures.Converters;
using Dental.Models;
using Dental.Models.Base;
using Dental.Models.Templates;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.ViewModels
{
    public class TreeWrapperViewModel<T> : BaseWrapperViewModel<T> where T : BaseTemplate<T>
    {
        public delegate void SaveCommand(T m, bool p);
        public event SaveCommand EventSave;

        public ObservableCollection<T> Collection { get; set; }
        private bool isSelectedVal = false;

        [Command]
        public void SelectItemInField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.CurrentItem is T item)
                    {
                        //if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = item;
                        Copy.Parent = item;
                        Copy.ParentId = item.Id;
                        isSelectedVal = true;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

            }
        }

        [Command]
        public void ClearSelectField(object p)
        {
            try
            {
                if (p is PopupBaseEdit popup)  popup.EditValue = null;          
            }
            catch
            {

            }
        }

        [Command]
        public void Save(object p)
        {
            try
            {
                foreach(var property in typeof(T).GetProperties())
                {
                    if (property.GetCustomAttributes(false)?.Where(f => f is ClonableAttribute)?.Count() > 0) 
                        property.SetValue(Model, property.GetValue(Copy));
                }

                EventSave?.Invoke(Model, isSelectedVal);

                if (p is Window win) win?.Close();
            }
            catch
            {
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "При сохранении значения в базу данных произошла ошибка!",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error
                );
            }
        }
 
    }
}
