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

            foreach (var assembly in data.Assemblys)
                data.PushElement(assembly.DefinedTypes.First(t => t.Name == typename));

            data.Receiver = data.Element;

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushElement();
        }
    }
}
