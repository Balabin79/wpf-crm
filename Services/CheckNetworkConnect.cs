using B6CRM.Models;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services
{
    internal static class CheckNetworkConnect
    {
        public static bool IsConnectSuccess(string ip_address, int port_number = 80, int timeout = 7)
        {
            try
            {
                if (!new TcpClient().ConnectAsync(ip_address, port_number).Wait(timeout * 1000))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
