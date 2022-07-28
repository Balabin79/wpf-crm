using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public static class QueueStatuses
    {
        public enum EventType { Added = 0, Edited = 1, Removed = 2 };
        public  enum SendingStatus { New = 0, Sended = 1, Error = 2 };
    }
}