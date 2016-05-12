﻿using fun.Core;
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

        public NetworkProcessHostElement(Environment environment, Entity entity)
            : base(environment, entity)
        {

        }

        public override void Initialize()
        {
            udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public override void OnClose()
        {
            udp.Close();
        }
    }
}