using fun.IO.Data;
using System.Xml;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.IO.XmlParsers
{
    internal sealed class EnvironmentXmlParser : XmlParser
    {
        private IEnvironmentDataStore data;

        public EnvironmentXmlParser(IEnvironmentDataStore data)
        {
            this.data = data;

            parsers = new XmlParser[] 
            {
                new EntityXmlParser(data),
                new LibaryXmlParser(data)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == typeof(Environment).Name;
        }

        public override void Parse(XmlNode node)
        {
            data.PushEnvironment();
            data.AddPushedEnvirionment();

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushEnvironment();
        }
    }
}
