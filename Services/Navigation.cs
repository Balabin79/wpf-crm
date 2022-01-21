using Dental.Infrastructures.Commands.Base;
using Dental.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Dental.Views.Pages.UserControls;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Data.Entity;
using Dental.Models;
using System.Collections.ObjectModel;

namespace Dental.Services
{
    sealed public class Navigation : ViewModelBase
    {
        private static  Navigator instance;

        static Navigation() {}
        private Navigation() { }

        public static Navigator Instance
        {
            get {
                if (instance == null)
                {
                    instance = new Navigator();
                    return instance;
                }
                    
                return instance;   
            }

        }


    }
}
