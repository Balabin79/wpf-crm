namespace Dental.Models.Base
{
    class Connection
    {
        private static Connection instance;
        public ApplicationContext Db { get; set; }

        private Connection()
        { }

        public static Connection getInstance()
        {
            if (instance == null)
            {
                instance = new Connection();
                instance.Db = new ApplicationContext();
            }               
            return instance;
        }
    }
}
