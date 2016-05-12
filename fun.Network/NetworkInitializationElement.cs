using fun.Core;
using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
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
        private TcpListener tcp;
        private int clientCount;
        private int bufferSize;

        public int Port;

        public NetworkInitializationElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            tcp = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            tcp.Start();
            tcp.BeginAcceptTcpClient(HandleNewClient, null);
            //udp = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
            //udp.BeginReceive(new AsyncCallback(HandleNewClient), null);
            clientCount = 0;
            bufferSize = 2;
        }

        private void HandleNewClient(IAsyncResult res)
        {
            var client = tcp.EndAcceptTcpClient(res);

            var data = new byte[bufferSize];
            client.GetStream().BeginRead(data, 0, data.Length, ReadClientStream, new { Client = client, Data = data });
            tcp.BeginAcceptTcpClient(HandleNewClient, null);
        }

        private void ReadClientStream(IAsyncResult res)
        {
            var client = (TcpClient)res.AsyncState.GetType().GetProperty("Client").GetValue(res.AsyncState);
            var data = (byte[])res.AsyncState.GetType().GetProperty("Data").GetValue(res.AsyncState);

            try { client.GetStream().EndRead(res); }
            catch (IOException) { return; }

            if (data.Last() != 0)
            {
                var _data = new byte[data.Length + bufferSize];
                client.GetStream().BeginRead(_data, 0, _data.Length, ReadClientStream, new { Client = client, Data = data });
                return;
            }

            var sender = new IPEndPoint(IPAddress.Any, 0);
            var request = Encoding.UTF8.GetString(data).Trim('\0');
            Console.WriteLine(request);
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
            foreach (var entity in Environment.Entities)
            {
                if (entity.ContainsElement<NetworkInitializationElement>())
                    continue;
                if (entity.Name == clientPlayer.Name)
                    continue;

                env.AddEntity(entity);
            }

            clientCount++;
            var next_data = new byte[bufferSize];
            client.GetStream().BeginRead(next_data, 0, next_data.Length, ReadClientStream, new { Client = client, Data = data });
        }
    }
}
