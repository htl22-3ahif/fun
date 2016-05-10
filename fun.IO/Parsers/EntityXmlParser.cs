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
            var enable = true;
            if (node.Attributes["Enable"] != null)
                enable = bool.Parse(node.Attributes["Enable"].Value);

            data.PushEntity(name);
            data.AddPushedEntity();
            data.Entity.Enable = enable;

            foreach (var _node in node.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

            data.DepushEntity();
        }
    }
}
