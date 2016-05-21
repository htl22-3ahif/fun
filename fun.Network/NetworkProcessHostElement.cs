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

        public IPEndPoint ClientEndPoint;
        public string[] PerceiveableEntities;

        public NetworkProcessHostElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
        }

        public override void Initialize()
        {
            var sender = new IPEndPoint(0, 0);
            udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            udp.Connect(ClientEndPoint);
            udp.Client.ReceiveTimeout = 1000;

            udp.Send(NetworkInitializationElement.CHECK_HOST, NetworkInitializationElement.CHECK_HOST.Length);
            var received = udp.Receive(ref sender);
            
            if (received != NetworkInitializationElement.CHECK_CLIENT)
            {
                Console.WriteLine("The user does not reply D: i think we should disconnect");
                Environment.RemoveEntity(Entity.Name);
                udp.Close();
            }
            else
                Console.WriteLine("The user wants to play with us! yay");

        }

        public override void Update(double time)
        {

        }

        public override void OnClose()
        {
            udp.Close();
        }
    }
}
