using Dental.Infrastructures.Commands.Base;
using Dental.Models;
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

namespace Dental.Services
{
    sealed class Db 
    {
        private static readonly Db instance = new Db();

        static Db(){}
        private Db(){}

        public static Db Instance
        {
            get
            {
                return instance;
            }
        }

        public ApplicationContext Context { get; } = new ApplicationContext();

    }
}
