using System;

namespace Dental.Infrastructures.Logs
{
    class ConvertorLog
    {
        private Exception RepositoryException { get; set; }

        public ConvertorLog(Exception e)
        {
            RepositoryException = e;
        }

        public void run()
        {

        }
    }
}
