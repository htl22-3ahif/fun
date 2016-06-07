using fun.Core;
using fun.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Network
{
    public sealed class NetworkProcessHostElement : Element
    {
        private UdpClient udp;
        private TcpClient tcp;
        private double delta;

        public int FromPort { get { return (udp.Client.LocalEndPoint as IPEndPoint).Port; } }

        //public IPEndPoint ClientEndPoint;
        public string ToIP;
        public int ToPort;
        public string[] PerceiveableEntities;

        public NetworkProcessHostElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
        }

        public override void Initialize()
        {
            delta = 0;
            udp = new UdpClient();

            // connecting to the client
            udp.Connect(new IPEndPoint(IPAddress.Parse(ToIP), ToPort));


            new Task(() => { while (true) HandleClientPackets(); }).Start();
        }

        public void AcceptClient()
        {
            var listener = new TcpListener(IPAddress.Any, FromPort);
            try
            {
                listener.Start();
                tcp = listener.AcceptTcpClient();
            }
            finally { listener.Stop(); }
        }

        public override void Update(double time)
        {
            // now we get to the process
            // for the beginning, I just want to send changes to the client
            // and only of the TransformElement and only the Position field (hardcoded)
            // we surely will replace that by a better solution for the future

            // since we do not recognize if the value was changed or not
            // and so we will probably spam the client pretty hard
            // we will add a timer, after a defined timespan is passed, we will send again

            delta += time;
            // if one second has not passed
            if (delta < 1)
                // just end here
                return;

            // else reset the timer
            delta = 0;

            // looping through all entities, the client is authorized to perceive
            foreach (var ename in PerceiveableEntities)
            {
                // getting the entity from name
                var entity = Environment.GetEntity(ename);

                // checking if entity contains the TransformElement (hardcoded)
                if (!entity.Elements.Any(e => e.GetType().Name == "TransformElement"))
                    // if he does not then
                    // looping to next
                    continue;

                // getting the transform reference
                var transform = entity.Elements.First(e => e.GetType().Name == "TransformElement");

                // getting the transform.position field
                var position = transform.GetType().GetField("Position").GetValue(transform);

                // getting the x y and z valies
                var x = position.GetType().GetField("X").GetValue(position);
                var y = position.GetType().GetField("Y").GetValue(position);
                var z = position.GetType().GetField("Z").GetValue(position);

                // creating a really simple packet
                var message = Encoding.UTF8.GetBytes(string.Format("Entity:{0}\nX:{1}\nY:{2}\nZ:{3}\n", ename, x, y, z));

                Console.WriteLine("Sending message: " + string.Format("Entity:{0}\nX:{1}\nY:{2}\nZ:{3}\n", ename, x, y, z));

                // sending the message async
                udp.BeginSend(message, message.Length, null, null);
            }
        }
        public override void OnEntityAdded(Entity entity)
        {
            var net = tcp.GetStream();

            new Task(() => new EnvironmentXmlWriter().Save(net, Environment, "fun.Basics.dll", "fun.Network.dll")).Start();
        }
        public void HandleClientPackets()
        {
            try
            {
                // here we will receive informations about the client's entity

                var sender = new IPEndPoint(0, 0);
                // getting the data and where its from
              
                var data = udp.Receive(ref sender);
                // encode the packet
                
                var message = Encoding.UTF8.GetString(data);

                Console.WriteLine("Got a message: " + message);

                var str = message.Split(';');
                var x = float.Parse(str[0]);
                var y = float.Parse(str[1]);
                var z = float.Parse(str[2]);

                var transform = Entity.Elements.First(e => e.GetType().Name == "TransformElement");
                var position = transform.GetType().GetField("Position").GetValue(transform);
                position.GetType().GetField("X").SetValue(position, x);
                position.GetType().GetField("Y").SetValue(position, y);
                position.GetType().GetField("Z").SetValue(position, z);
            }
            catch (SocketException)
            {
                // TODO: change the handleing, if nothing is received
                // since he loves to throw exceptions instead of waiting for a packet
                // idc
            }
        }

        public override void OnClose()
        {
            udp.Close();
        }
    }
}
