using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;

namespace Dental.Services
{
    public class PingService
    {
        private readonly string host = "google.com";
        public bool IsNetworkAvailable()
        {
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
                return false;
            }
        }
    }
}
