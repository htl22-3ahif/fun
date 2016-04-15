using System;
using System.Xml;
using fun.IO.Data;

namespace fun.IO.XmlParsers
{
    internal class ArrayXmlParser : XmlParser
    {
        private IPropertyDataStore data;

        public ArrayXmlParser(IPropertyDataStore data)
        {
            this.data = data;
        }

        public override bool TryParse(XmlNode node)
        {
            return Type.GetType(node.Name).IsArray;
        }

        public override void Parse(XmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}