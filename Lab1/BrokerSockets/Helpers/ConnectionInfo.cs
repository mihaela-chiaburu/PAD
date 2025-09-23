using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class ConnectionInfo
    {
        public Socket socket { get; set; }
        public string Adress { get; set; }
        public string Topic { get; set; }
        public byte[] Data { get; set; }
        public const int BUFF_SIZE = 1024;

        public ConnectionInfo()
        {
            Data = new byte[BUFF_SIZE];
        }

    }
}
