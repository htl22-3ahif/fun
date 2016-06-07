using fun.Core;
using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
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
        private TcpClient tcp;
        private double delta;

        public int FromPort { get; set; }

        public string IP;
        public int Port;

        public NetworkProcessClientElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            udp = new UdpClient();
            var host = new IPEndPoint(IPAddress.Parse(IP), Port);
            udp.Connect(host);

            Console.WriteLine(udp.Client.LocalEndPoint.ToString() + udp.Client.RemoteEndPoint.ToString());

            tcp = new TcpClient(udp.Client.LocalEndPoint as IPEndPoint);
            tcp.Connect(host);

            new Task(() => { while (true) HandlePacketUdp(); }).Start();
            new Task(() => { while (true) HandlePacketTcp(); }).Start();
        }

        public override void Update(double time)
        {
            delta += time;
            // if one second has not passed
            if (delta < 1)
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
            var message = Encoding.UTF8.GetBytes(string.Format("X:{0}\nY:{1}\nZ:{2}\n", x, y, z));

            // sending the message async
            udp.BeginSend(message, message.Length, null, null);
        }

        private void HandlePacketUdp()
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
        private void HandlePacketTcp()
        {
            var net = tcp.GetStream();
            var mem = new MemoryStream();

            // creating a 4k chunck
            var data = new byte[4096];

            // getting the data and its length
            var length = net.Read(data, 0, data.Length);

            if (length == 0)
                return;

            // now the critical part is coming
            try
            {
                // while the received bytes count is not null
                while (length > 0)
                {
                    // writing the data in an own stream (memorystream)
                    mem.Write(data, 0, length);

                    // after that, again getting the data and its length
                    length = net.Read(data, 0, data.Length);
                }
                // at the end, it has to happen an receive time out exception
                // thats the very moment, where the host if finished with sending
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

            string[] libraries;
            mem.Position = 0;
            var newEnv = new EnvironmentXmlReader().Load(mem, out libraries)[0];

            foreach (var newEntity in newEnv.Entities)
            {
                if (!Environment.Entities.Any(e => e.Name == newEntity.Name))
                    Environment.AddEntity(newEntity);
            }
        }
    }
}
