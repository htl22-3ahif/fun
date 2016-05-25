using fun.IO;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fun.Client.Components
{
    internal sealed class NetworkComponent : GameComponent
    {
        private TcpClient tcp;
        private UdpClient udp;

        public NetworkComponent(GameWindow game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            // yea, its a really complex way of reading data from an networkstream
            // but, even if its hard to belive, its one of the shortest and efficient methods
            // so please excuse for the pretty long spaghetti

            string[] libaries;
            tcp = new TcpClient();
            udp = new UdpClient();

            tcp.Connect(IPAddress.Parse("127.0.0.1"), 844);
            tcp.Client.ReceiveTimeout = 20;

            var net = tcp.GetStream();
            var mem = new MemoryStream();
            var message = Encoding.UTF8.GetBytes("nyaa~");

            net.Write(message, 0, message.Length);

            var data = new byte[4096];
            var length = net.Read(data, 0, data.Length);
            try
            {
                while (length > 0)
                {
                    mem.Write(data, 0, length);
                    length = net.Read(data, 0, data.Length);
                }
            }
            catch (IOException e)
            {
                // if the ReceiveTimeout is reached an IOException will be raised...
                // with an InnerException of type SocketException and ErrorCode 10060
                var socketExept = e.InnerException as SocketException;
                if (socketExept == null || socketExept.ErrorCode != 10060)
                    // if it's not the "expected" exception, let's not hide the error
                    throw e;
            }
            
            // IMPORTANT!!! set the position to zero
            // to avoid sending only a part of the stream or at worst,
            // when the position is situated at the end of the stream
            // you are sending nothing
            // so its really important, and it takes long to find the error
            mem.Position = 0;
            var environment = new EnvironmentXmlReader().Load(mem, out libaries)[0];
        }
    }
}
