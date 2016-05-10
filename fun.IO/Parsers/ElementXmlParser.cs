using fun.Core;
using fun.IO.Data;
using System;
using System.Xml;
using System.Linq;

namespace fun.IO.XmlParsers
{
    internal sealed class ElementXmlParser : XmlParser
    {
        private IElementDataStore data;

        public ElementXmlParser(IElementDataStore data)
        {
            this.data = data;
            this.parsers = new XmlParser[]
            {
                new FieldXmlParser(data)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == typeof(Element).Name;
        }

        public override void Parse(XmlNode node)
        {
            var typename = node.Attributes[typeof(Type).Name].Value;
            var enable = true;
            if (node.Attributes["Enable"] != null)
                enable = bool.Parse(node.Attributes["Enable"].Value);
            Type type = null;

            foreach (var assembly in data.Assemblys)
            {
                type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == typename);
                if (type != default(Type))
                    break;
            }

            if (type == default(Type))
                // TODO: Write message
                throw new XmlException();

            data.PushElement(type);
            data.Receiver = data.Element;
            data.Element.Enable = enable;

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushElement();
        }
    }
}
