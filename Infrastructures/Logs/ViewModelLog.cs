using System;

namespace Dental.Infrastructures.Logs
{
    class ViewModelLog
    {
        private Exception ViewModelException { get; set; }

        public ViewModelLog(Exception e)
        {
            ViewModelException = e;
        }

        public void run()
        {

        }
    }
}
