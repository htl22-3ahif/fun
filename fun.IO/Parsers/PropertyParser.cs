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
        IElementPropertyDataStore data;

        public PropertyParser(IElementPropertyDataStore data)
        {
            this.data = data;
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Property";
        }

        public override void Parse(XmlNode node)
        {
            var name = node.Attributes["name"].Value;
            var property = data.Element.GetType().GetProperty(name);
            var values = node.Attributes["value"].Value.Split('/');

            if (values.Length == 1 && property.PropertyType.IsSubclassOf(typeof(ValueType)))
            {
                property.SetValue(data.Element, Convert.ChangeType(values[0], property.PropertyType));
                return;
            }

            var constructors = property.PropertyType.GetConstructors()
                .Where(c => c.GetParameters().Length == values.Length);

            foreach (var constructor in constructors)
            {
                var convertedValues = new object[values.Length];
                for (int i = 0; i < values.Length; i++)
                    convertedValues[i] = Convert.ChangeType(values[i], constructor.GetParameters()[i].ParameterType);

                var result = Activator.CreateInstance(property.PropertyType, convertedValues);
                property.SetValue(data.Element, result);
            }
        }
    }
}
