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
        private bool completed;
        private IAsyncResult async;

        public int Port;
        
        public NetworkInitializationElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            udp = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
            completed = true;
        }

        public override void Update(double time)
        {
            if (completed)
                async = udp.BeginReceive(new AsyncCallback(HandleNewClient), null);
            completed = async.IsCompleted;
        }

        private void HandleNewClient(IAsyncResult res)
        {
            var sender = new IPEndPoint(IPAddress.Any, 0);
            var data = udp.EndReceive(res, ref sender);
            Console.WriteLine(Encoding.ASCII.GetString(data) + " was sent by " + sender.ToString());

            var highPort = GetFreePort();

            var player = Environment.GetEntity("Player");
            var hostPlayer = new Entity("Player" + highPort, Environment);
            foreach (var element in player.Elements)
            {
                hostPlayer.AddElement(element.GetType());
                var clientElement = hostPlayer.GetElement(element.GetType());
                foreach (var field in element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                    field.SetValue(clientElement, field.GetValue(element));
            }
            hostPlayer.GetElement<NetworkProcessHostElement>().EndPoint = sender;
            hostPlayer.Initialize();

            var env = new Environment();
            var clientPlayer = new Entity("Player", env);
            foreach (var element in player.Elements)
            {
                clientPlayer.AddElement(element.GetType());
                var clientElement = clientPlayer.GetElement(element.GetType());
                foreach (var field in element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                    field.SetValue(clientElement, field.GetValue(element));
            }
            clientPlayer.RemoveElement(typeof(NetworkProcessHostElement));
            clientPlayer.AddElement<NetworkProcessClientElement>();
            env.AddEntity(clientPlayer);
        }

        private int GetFreePort()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return (socket.LocalEndPoint as IPEndPoint).Port;
        }
    }
}
