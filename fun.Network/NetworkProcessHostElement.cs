using fun.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Network
{
    public sealed class NetworkProcessHostElement : Element
    {
        private UdpClient udp;
        private double delta;

        public IPEndPoint ClientEndPoint;
        public string[] PerceiveableEntities;

        public NetworkProcessHostElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
        }

        public override void Initialize()
        {
            delta = 0;
            var sender = new IPEndPoint(0, 0);
            udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            // connecting to the client
            udp.Connect(ClientEndPoint);

            // defining receive time out (not to wait infinitly long)
            udp.Client.ReceiveTimeout = 1000;

            // sending the defined code to make sure the client is connected too
            udp.Send(NetworkInitializationElement.CHECK_HOST, NetworkInitializationElement.CHECK_HOST.Length);

            // waiting for receive
            var received = udp.Receive(ref sender);
            
            // checking if the response of client maches with the defined response
            if (received != NetworkInitializationElement.CHECK_CLIENT)
            {
                // if not then
                // writing a message so you understand that the client does not connect (may be replaced by log)
                Console.WriteLine("The user does not reply D: i think we should disconnect");

                // since the client is not responding correctily, we are removing his entity
                Environment.RemoveEntity(Entity.Name);

                // closing the connection
                udp.Close();
            }
            else
                // if it matches then
                // writing a message so you understand that the client does connect (may be replaced by log)
                Console.WriteLine("The user wants to play with us! yay");

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
            if (delta < 1000)
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

                // sending the message async
                udp.BeginSend(message, message.Length, null, null);
            }
        }

        public override void OnClose()
        {
            udp.Close();
        }
    }
}
