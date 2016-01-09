using fun.IO.Data;
using System.Xml;

namespace fun.IO.Parsers
{
    internal sealed class StringParser : Parser
    {
        private IPropertyDataStore data;

        public StringParser(IPropertyDataStore data)
        {
            this.data = data;
            parsers = null;
        }

        public override bool TryParse(XmlNode node)
        {
            return data.Element.GetType().GetProperty(node.Name).PropertyType == typeof(string);
        }

        public override void Parse(XmlNode node)
        {
            var prop = data.Element.GetType().GetProperty(node.Name);
            prop.SetValue(data.Element, node.Value);
        }
    }
}
