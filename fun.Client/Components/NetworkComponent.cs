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
            string[] libaries;
            tcp = new TcpClient();
            udp = new UdpClient();

            var message = Encoding.UTF8.GetBytes("nyaa~");

            tcp.Connect(IPAddress.Parse("10.44.31.38"), 844);
            tcp.GetStream().Write(message, 0, message.Length);
        }
    }
}
