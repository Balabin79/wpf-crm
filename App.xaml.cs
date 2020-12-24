using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Dental.Repositories.CouchDb;

namespace Dental
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        App()
        {
            test();
        } 



        public async void test()
        {
            QueryDoc query = new QueryDoc();

            await query.GetDocument("doctor_3");
            return;
        }
    }
}
