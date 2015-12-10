using fun.Core;
using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO.Parsers
{
    internal sealed class LibaryParser : Parser
    {
        private ILibaryDataStore data;

        public LibaryParser(ILibaryDataStore data)
        {
            this.data = data;
            parsers = null;
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Lib";
        }

        public override void Parse(XmlNode node)
        {
            foreach (var att in node.Attributes.OfType<XmlNode>())
                att.Value = att.Value.ToLower();

            var path = node.Attributes["Path"].Value;
            var assembly = Assembly.LoadFrom(path);

            data.AddLibary(assembly);
        }
    }
}
