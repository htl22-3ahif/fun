using fun.Core;
using fun.IO.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
                new Vector3Parser(data),
                new StringParser(data)
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

            foreach (var att in node.Attributes.OfType<XmlAttribute>())
                if (att.Name != typeof(Type).Name)
                    foreach (var parser in parsers)
                        if (parser.TryParse(att))
                            parser.Parse(att);

            data.DepushElement();
        }
    }
}
