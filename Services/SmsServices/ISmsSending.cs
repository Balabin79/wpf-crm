using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices
{
    public interface ISmsSending
    {
        Task<HttpResponseMessage> SendMsg(string text, string contacts);
    }
}
