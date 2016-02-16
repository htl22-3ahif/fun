using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.IO.XmlParsers
{
    internal sealed class PropertyXmlParser : XmlParser
    {
        IPropertyDataStore data;

        public PropertyXmlParser(IPropertyDataStore data)
        {
            this.data = data;
            this.parsers = new XmlParser[]
            {
                new ParamXmlParser(data)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Property";
        }

        public override void Parse(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;
            var property = data.Element.GetType().GetProperty(name);

            //alle params durchgehen (in params object array stored)
            foreach (var _node in node.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            if (property.PropertyType.IsPrimitive)
                property.SetValue(data.Element, data.Params.First());
            else
            {
                var propertyObject = Activator.CreateInstance(property.PropertyType, data.Params);
                property.SetValue(data.Element, propertyObject);
            }

            data.FlushParams();
        }
    }
}
