using fun.Core;
using fun.IO.Data;
using System.Linq;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO.XmlParsers
{
    internal sealed class EntityXmlParser : XmlParser
    {
        private IEntityDataStore data;

        public EntityXmlParser(IEntityDataStore data)
        {
            this.data = data;

            parsers = new XmlParser[]
            {
                new ElementXmlParser(data)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == typeof(Entity).Name;
        }

        public override void Parse(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;

            data.PushEntity(name);
            data.AddPushedEntity();

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushEntity();
        }
    }
}
