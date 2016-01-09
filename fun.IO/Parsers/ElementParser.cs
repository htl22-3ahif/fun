using fun.Core;
using fun.IO.Data;
using System;
using System.Xml;
using System.Linq;

namespace fun.IO.Parsers
{
    internal sealed class ElementParser : Parser
    {
        private IElementDataStore data;

        public ElementParser(IElementDataStore data)
        {
            this.data = data;
            this.parsers = new Parser[]
            {
                new PropertyParser(data)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == typeof(Element).Name;
        }

        public override void Parse(XmlNode node)
        {
            var typename = node.Attributes[typeof(Type).Name].Value;
            var props = node.Attributes.OfType<XmlAttribute>().Where(a => a.Value != typeof(Type).Name);
            var lastElement = data.Element;

            foreach (var assembly in data.Assemblys)
                foreach (var type in assembly.ExportedTypes)
                    if (type.Name == typename)
                        data.PushElement(type);

            if (data.Element == null)
                throw new XmlException(typename + " does not exist.");

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushElement();
        }
    }
}
