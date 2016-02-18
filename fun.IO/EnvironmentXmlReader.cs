using fun.IO.Data;
using fun.IO.XmlParsers;
using System.IO;
using System.Xml;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.IO
{
    public sealed class EnvironmentXmlReader
    {
        private DataStore data;
        private XmlParser[] parsers;

        public EnvironmentXmlReader()
        {
            data = new DataStore();
            parsers = new XmlParser[]
            {
                new EnvironmentXmlParser(data)
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
