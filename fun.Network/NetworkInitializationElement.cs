using fun.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Network
{
    public sealed class NetworkInitializationElement : Element
    {
        private UdpClient udp;
        private int clientCount;

        public int Port;
        
        public NetworkInitializationElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            udp = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
            udp.BeginReceive(new AsyncCallback(HandleNewClient), null);
            clientCount = 0;
        }

        public override void Update(double time)
        {
        }

        private void HandleNewClient(IAsyncResult res)
        {
            var sender = new IPEndPoint(IPAddress.Any, 0);
            var data = udp.EndReceive(res, ref sender);
            Console.WriteLine(Encoding.ASCII.GetString(data) + " was sent by " + sender.ToString());

            var player = Environment.GetEntity("Player");
            var hostPlayer = new Entity("Player" + clientCount, Environment);
            foreach (var element in player.Elements)
            {
                hostPlayer.AddElement(element.GetType());
                var clientElement = hostPlayer.GetElement(element.GetType());
                foreach (var field in element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                    field.SetValue(clientElement, field.GetValue(element));
            }
            hostPlayer.AddElement<NetworkProcessHostElement>();
            hostPlayer.Initialize();
            Environment.AddEntity(hostPlayer);

            var env = new Environment();
            var clientPlayer = new Entity("Player", env);
            foreach (var element in hostPlayer.Elements)
            {
                clientPlayer.AddElement(element.GetType());
                var clientElement = clientPlayer.GetElement(element.GetType());
                foreach (var field in element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                    field.SetValue(clientElement, field.GetValue(element));
            }
            clientPlayer.RemoveElement(typeof(NetworkProcessHostElement));
            clientPlayer.AddElement<NetworkProcessClientElement>();
            env.AddEntity(clientPlayer);

            clientCount++;
            udp.BeginReceive(new AsyncCallback(HandleNewClient), null);
        }

        private int GetFreePort()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return (socket.RemoteEndPoint as IPEndPoint).Port;
        }
    }
}
