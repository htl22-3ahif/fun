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
    public sealed class NetworkProcessClientElement : Element
    {
        private UdpClient udp;
        private double delta;

        public string IP;
        public int Port;

        public NetworkProcessClientElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            udp = new UdpClient();
            udp.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));

            new Task(() => { while (true) HandleClientPacket(); }).Start();
        }

        public override void Update(double time)
        {
            delta += time;
            // if one second has not passed
            if (delta < 1000)
                // just end here
                return;

            // else reset the timer
            delta = 0;

            // checking if entity contains the TransformElement (hardcoded)
            if (!Entity.Elements.Any(e => e.GetType().Name == "TransformElement"))
                // if he does not then
                // looping to next
                return;

            // getting the transform reference
            var transform = Entity.Elements.First(e => e.GetType().Name == "TransformElement");

            // getting the transform.position field
            var position = transform.GetType().GetField("Position").GetValue(transform);

            // getting the x y and z valies
            var x = position.GetType().GetField("X").GetValue(position);
            var y = position.GetType().GetField("Y").GetValue(position);
            var z = position.GetType().GetField("Z").GetValue(position);

            // creating a really simple packet
            var message = Encoding.UTF8.GetBytes(string.Format("X:{1}\nY:{2}\nZ:{3}\n", x, y, z));

            // sending the message async
            udp.BeginSend(message, message.Length, null, null);
        }

        private void HandleClientPacket()
        {
            try
            {
                // here we will receive informations about the client's entity

                var sender = new IPEndPoint(0, 0);
                // getting the data and where its from

                var data = udp.Receive(ref sender);
                // encode the packet

                var message = Encoding.UTF8.GetString(data);

                var str = message.Split(';');
                var ename = str[0];
                var x = float.Parse(str[1]);
                var y = float.Parse(str[2]);
                var z = float.Parse(str[3]);

                var entity = Environment.GetEntity(ename);

                var transform = entity.Elements.First(e => e.GetType().Name == "TransformElement");
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
    }
}
