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
        public int BufferSize;

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
        }

        private void HandleNewClient(IAsyncResult res)
        {
            var client = tcp.EndAcceptTcpClient(res);

            var data = new byte[BufferSize];
            client.GetStream().BeginRead(data, 0, data.Length, ReadClientStream, new { Client = client, Data = data });
            tcp.BeginAcceptTcpClient(HandleNewClient, null);
        }

        private void ReadClientStream(IAsyncResult res)
        {
            // get the affected client and the received data
            var client = (TcpClient)res.AsyncState.GetType().GetProperty("Client").GetValue(res.AsyncState);
            var data = (byte[])res.AsyncState.GetType().GetProperty("Data").GetValue(res.AsyncState);

            // if the data is empty (filled with zeros)
            if (!data.Any(b => b != 0))
                // just get outta here
                return;

            // if stream is already at end, exit
            try { client.GetStream().EndRead(res); }
            catch (IOException) { return; }

            // if the chunck's size is not enough to respresent the whole data
            if (data.Last() != 0)
            {
                // the data's size will be increazed
                var _data = new byte[data.Length + BufferSize];

                // the data will be stored in the async state
                res.AsyncState.GetType().GetProperty("Data").SetValue(res.AsyncState, _data);

                // now the begin read async method will be launched
                // it will call the same method, but the chunck will be larger
                // there is the posibility, that the size still is not enough
                // in this case, he will just do the same again
                client.GetStream().BeginRead(_data, 0, _data.Length, new AsyncCallback(ReadClientStream), res.AsyncState);

                // exit from this method
                return;
            }

            // defining the sender (client's destination)
            var sender = new IPEndPoint(IPAddress.Any, 0);

            // converting the data into a string encoded in UTF8
            var request = Encoding.UTF8.GetString(data).Trim('\0');

            if (request != "nyaa~")
                return;

            // showing the request
            Console.WriteLine("Someone wants to play with us! nyaa~");

            // geting the shema for the client's entity
            var player = Environment.GetEntity("Player");

            // creating the player that will stay in the host's environment
            var hostPlayer = new Entity("Player" + sender.Address.ToString(), Environment);

            // applying the shema's elements to the host's entity
            ApplyElementsTo(player, hostPlayer);

            // adding the element that handles the networking process on the host side
            hostPlayer.AddElement<NetworkProcessHostElement>();

            // and finally, we are adding it to our environment
            Environment.AddEntity(hostPlayer);

            // after modifying the environment on the host's side
            // we now have to define the environment, which will be send to the client

            // contructing the new environment
            var env = new Environment();

            // constructing the new Entity, the client is authorized to control
            var clientPlayer = new Entity("Player", env);

            // applying the elements of the shema to the client's entity
            ApplyElementsTo(player, clientPlayer);

            // adding the elements that handles the networking on the client's side
            clientPlayer.AddElement<NetworkProcessClientElement>();

            // adding the client's entity to his environment
            env.AddEntity(clientPlayer);

            // now add all the other entities the client is authorized to perceive
            foreach (var entity in Environment.Entities)
            {
                // of course, we won't send the entity that contains the network initialization element
                if (entity.ContainsElement<NetworkInitializationElement>())
                    continue;

                // and it won't add itself, because it is already added
                if (entity.Name == clientPlayer.Name)
                    continue;

                // adding the entity
                env.AddEntity(entity);
            }

            new EnvironmentXmlWriter().Save(client.GetStream(), env, new[] { "fun.Basics.dll", "fun.Network.dll" });

            // inizializing the whole entity
            hostPlayer.Initialize();

            // then we will set the new data buffer for the next package to be received
            // we do it because it may be that our current data buffer is bigger than the defined buffer size
            // this could be the case if we ran into the if at the begining
            var next_data = new byte[BufferSize];

            // again we are seting to wait for the next bytes to come in
            client.GetStream().BeginRead(next_data, 0, next_data.Length, ReadClientStream, new { Client = client, Data = next_data });
        }

        private void ApplyElementsTo(Entity source, Entity destination)
        {
            foreach (var element in source.Elements)
            {
                destination.AddElement(element.GetType());
                var clientElement = destination.GetElement(element.GetType());
                foreach (var field in element.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                    field.SetValue(clientElement, field.GetValue(element));
            }
        }
    }
}
