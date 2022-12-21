using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Windows.UI.Core;
using Windows.UI.Xaml.Documents;

namespace ConnectLan
{
    public class Connect
    {
        private string IpAddr;
        private int PortNumber;
        private IPEndPoint ip = null;
        private int TimeOut;

        public IPEndPoint Ip
        {
            get
            {
                if (ip == null)
                    ip = new IPEndPoint(IPAddress.Parse(this.IpAddr), this.PortNumber);
                return ip;
            }
            set { ip = value; }
        }

        public Connect(string instrIPAddress, int instrPortNo, int timeOut)
        {
            IpAddr = instrIPAddress;
            PortNumber = instrPortNo;
            TimeOut = timeOut;
            if (!Soket.Connected)
            {
                throw new SocketException();
            }  
        }

        public void WriteLine(string command)
        {
            Soket.Send(Encoding.ASCII.GetBytes(command + "\n"));
        }

        public string ReadLine()
        {
            byte[] data = new byte[1024];
            int receivedDataLength = Soket.Receive(data);
            return Encoding.ASCII.GetString(data, 0, receivedDataLength);
        }

        public byte[] ReadBytes()
        {
            byte[] data = new byte[1024];
            int receivedDataLength = Soket.Receive(data);
            return data;
        }

        Socket soket = null;

        public Socket Soket
        {
            get
            {
                if (soket == null)
                {
                    soket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    if (!soket.Connected)
                    {
                        soket.Connect(Ip);
                        soket.SendTimeout = TimeOut;
                        soket.ReceiveTimeout = TimeOut;
                    }
                }
                return soket;
            }
            set { soket = value; }
        }
    }
}
