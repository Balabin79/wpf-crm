using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Google
{
    public interface INotification
    {
        void ShowMsgError(string msg);
        void ShowSuccessMsg(string msg);
    }
}
