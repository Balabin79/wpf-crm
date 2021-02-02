using System;

namespace Dental.Infrastructures.Logs
{
    class RepositoryLog
    {
        private Exception RepositoryException { get; set; }

        public RepositoryLog(Exception e)
        {
            RepositoryException = e;
        }

        public void run()
        {

        }
    }
}
