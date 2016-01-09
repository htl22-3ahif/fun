using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.IO.Parsers
{
    internal sealed class PropertyParser : Parser
    {
        IPropertyDataStore data;

        public PropertyParser(IPropertyDataStore data)
        {
            this.data = data;
            this.parsers = new Parser[]
            {
                new ParamParser(data)
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

            var propertyObject = Activator.CreateInstance(property.PropertyType, data.Params);
            property.SetValue(data.Element, propertyObject);
            data.FlushParams();
        }
    }
}
