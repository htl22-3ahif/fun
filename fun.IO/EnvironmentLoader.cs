using fun.IO.Data;
using fun.IO.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO
{
    public sealed class EnvironmentLoader
    {
        private DataStore data;
        private Parser[] parsers;

        public EnvironmentLoader()
        {
            data = new DataStore();
            parsers = new Parser[]
            {
                new EnvironmentParser(data)
            };
        }

        public Environment[] Load(Stream input)
        {
            var doc = new XmlDocument();
            doc.Load(input);

            foreach (var node in doc.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(node))
                        parser.Parse(node);

            return data.Envionments;
        }
    }
}
