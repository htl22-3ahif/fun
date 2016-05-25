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
        public const int BUFFERSIZE = 4096;

        private TcpListener tcp;

        public int Port;
        public string BlueprintEntity;

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
            tcp.BeginAcceptTcpClient(HandleNewClient, null);

            var data = new byte[5];
            client.GetStream().BeginRead(data, 0, data.Length, ReadClientStream, new { Client = client, Data = data });
        }

        private void ReadClientStream(IAsyncResult res)
        {
            // get the affected client and the received data
            var client = (TcpClient)res.AsyncState.GetType().GetProperty("Client").GetValue(res.AsyncState);
            var data = (byte[])res.AsyncState.GetType().GetProperty("Data").GetValue(res.AsyncState);

            // converting the data into a string encoded in UTF8
            var request = Encoding.UTF8.GetString(data).Trim('\0');

            if (request != "nyaa~")
                return;

            // showing the request
            Console.WriteLine("Someone wants to play with us! nyaa~");

            // geting the shema for the client's entity
            var player = Environment.GetEntity(BlueprintEntity);

            // defining the sender (client's destination)
            var sender = client.Client.LocalEndPoint as IPEndPoint;

            // creating the player that will stay in the host's environment
            var hostPlayer = new Entity(BlueprintEntity + sender.Address.ToString(), Environment);

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
            var clientPlayer = new Entity(BlueprintEntity, env);

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

            // sending the environment to the client
            new EnvironmentXmlWriter().Save(client.GetStream(), env, new[] { "fun.Basics.dll", "fun.Network.dll" });

            // setting the perceiveable entities to get them sended to the client
            hostPlayer.GetElement<NetworkProcessHostElement>().PerceiveableEntities =
                env.Entities.Where(e => e.Name != clientPlayer.Name).Select(e => e.Name).ToArray();

            // inizializing the whole entity
            hostPlayer.Initialize();
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
