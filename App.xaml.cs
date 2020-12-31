using Dental.Repositories.CouchDb;
using System.Windows;

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
