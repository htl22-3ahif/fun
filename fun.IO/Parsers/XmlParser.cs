using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO.XmlParsers
{
    internal abstract class XmlParser
    {
        protected XmlParser[] parsers;

        public abstract bool TryParse(XmlNode node);
        public abstract void Parse(XmlNode node);
    }
}
