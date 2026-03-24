using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ConnectLan
{
    public class Connect
    {
        private string IpAddr  = "0.0.0.0";
        private int PortNumber = 0;
        private IPEndPoint ip  = null;
        private int TimeOut    = 1000;

        const int BufferSize = 16 * 1024;
        [ThreadStatic] static byte[] readBuffer;

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
            IpAddr     = instrIPAddress;
            PortNumber = instrPortNo;
            TimeOut    = timeOut;

            // Aciona conexão de imediato — lança SocketException se inacessível
            _ = Soket;
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

        private byte[] Read(int count)
        {
            byte[] buffer = new byte[count];
            int totalReceived = 0;

            while (totalReceived < count)
            {
                int received = Soket.Receive(buffer, totalReceived, count - totalReceived, SocketFlags.None);
                if (received == 0)
                    throw new SocketException(); // conexão encerrada pelo instrumento
                totalReceived += received;
            }

            return buffer;
        }

        private string ReadString(int count)
        {
            return Encoding.ASCII.GetString(Read(count));
        }

        public byte[] ReadBytes()
        {
            var buff = Read(1);

            if (buff[0] != (byte)'#')
                throw new Exception("O bloco binário não pôde ser interpretado.");

            int dataSizeLen = int.Parse(ReadString(1));
            int dataSize    = int.Parse(ReadString(dataSizeLen));

            return Read(dataSize);
        }

        Socket soket = null;

        public Socket Soket
        {
            get
            {
                if (soket == null)
                {
                    soket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    soket.Connect(Ip);
                    soket.SendTimeout    = TimeOut;
                    soket.ReceiveTimeout = TimeOut;
                }
                return soket;
            }
            set { soket = value; }
        }
    }
}
