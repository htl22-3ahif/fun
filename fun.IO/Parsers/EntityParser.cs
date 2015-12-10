using fun.Core;
using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO.Parsers
{
    internal sealed class EntityParser : Parser
    {
        private IEntityDataStore data;

        public EntityParser(IEntityDataStore data)
        {
            this.data = data;

            parsers = new Parser[]
            {
                new ElementParser(data)
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
