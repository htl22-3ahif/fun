using fun.IO.Data;
using System.Reflection;
using System.Xml;
using System.Linq;
using Environment = fun.Core.Environment;

namespace fun.IO.XmlParsers
{
    internal sealed class LibaryXmlParser : XmlParser
    {
        private ILibaryDataStore data;

        public LibaryXmlParser(ILibaryDataStore data)
        {
            this.data = data;
            parsers = null;
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Lib";
        }

        public override void Parse(XmlNode node)
        {
            foreach (var att in node.Attributes.OfType<XmlNode>())
                att.Value = att.Value.ToLower();

            var path = node.Attributes["Path"].Value;
            var assembly = Assembly.LoadFrom(path);

            data.AddLibary(assembly);
        }
    }
}
