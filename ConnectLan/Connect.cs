using System;
using System.Text;
using System.Net.Sockets;
using System.Net;


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
            string line;
            byte[] data = new byte[1024];
            int receivedDataLength = Soket.Receive(data);
            line = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            return line;
        }

        const int BufferSize = 16 * 1024;
        [ThreadStatic] static byte[] readBuffer;

        private byte[] Read(int count) 
        {
            byte[] buffer = new byte[count];
            int aux = 0;

            while (aux < count)
            {
                aux = Soket.Receive(buffer, aux, count, socketFlags: SocketFlags.None);
            }
            

            return buffer;
        }

        private string ReadString(int count)
        {
            return System.Text.Encoding.ASCII.GetString(Read(count));
        }

        public byte[] ReadBytes()
        {
            int dataSize;
            int dataSizeLen;
            byte[] dataBytes;

            var buff = Read(1);

            if (buff[0] != (byte)'#')
            {
                throw new Exception("The Binary Block could not be parsed.");
            }

            var digits = ReadString(1);
            dataSizeLen = int.Parse(digits);

            var length = ReadString(dataSizeLen);
            dataSize = int.Parse(length);

            dataBytes = Read(dataSize);


            return dataBytes;
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
