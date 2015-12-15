using fun.IO.Data;
using fun.IO.Parsers;
using System.IO;
using System.Xml;
using System.Linq;
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
