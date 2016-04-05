using System;
using System.Xml;
using fun.IO.Data;

namespace fun.IO.XmlParsers
{
    internal class StringXmlParser : XmlParser
    {
        private IPropertyDataStore data;

        public StringXmlParser(IPropertyDataStore data)
        {
            this.data = data;
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == typeof(string).FullName;
        }

        public override void Parse(XmlNode node)
        {
            data.Receiver = node.InnerText;
        }
    }
}