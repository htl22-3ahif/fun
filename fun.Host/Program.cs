using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fun.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] libraries;
            var environment = new EnvironmentXmlReader().Load(new FileStream(args[0], FileMode.Open), out libraries)[0];
            environment.Initialize();
            while (true)
            {
                Thread.Sleep(20);
                environment.Update(20);
            }
        }
    }
}
